#!/bin/bash

# ============================================================
# 🚀 API PERFORMANCE TEST SCRIPT
#   Simulação de Crédito - Hackathon
# ============================================================

set -e

# Configurações
BASE_URL="http://localhost:8080"
RESULTS_DIR="./test-results"
TIMESTAMP=$(date +"%Y%m%d_%H%M%S")
LOG_FILE="$RESULTS_DIR/performance_test_$TIMESTAMP.log"

# Cores para output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
PURPLE='\033[0;35m'
CYAN='\033[0;36m'
NC='\033[0m' # No Color

# Função para log
log() {
    echo -e "${2:-$BLUE}$1${NC}" | tee -a "$LOG_FILE"
}

# Função para verificar se a API está rodando
check_api_health() {
    log "🔍 Verificando saúde da API..." "$CYAN"
    
    if curl -s -f "$BASE_URL/health" > /dev/null; then
        log "✅ API está respondendo" "$GREEN"
        return 0
    else
        log "❌ API não está respondendo em $BASE_URL" "$RED"
        return 1
    fi
}

# Função para teste de baseline (1 usuário)
baseline_test() {
    log "📊 Executando teste de baseline (1 usuário, 30s)..." "$PURPLE"
    
    local output_file="$RESULTS_DIR/baseline_$TIMESTAMP.json"
    
    k6 run \
        --out json="$output_file" \
        --env BASE_URL="$BASE_URL" \
        --env TEST_TYPE="baseline" \
        --env USERS=1 \
        --env DURATION=30 \
        --env RAMP_UP=5 \
        tests/performance/baseline.js
    
    log "✅ Teste de baseline concluído: $output_file" "$GREEN"
}

# Função para teste de carga
load_test() {
    local users=$1
    local duration=$2
    local ramp_up=${3:-10}
    
    log "🔥 Executando teste de carga ($users usuários, ${duration}s)..." "$PURPLE"
    
    local output_file="$RESULTS_DIR/load_${users}users_${duration}s_$TIMESTAMP.json"
    
    k6 run \
        --out json="$output_file" \
        --env BASE_URL="$BASE_URL" \
        --env TEST_TYPE="load" \
        --env USERS="$users" \
        --env DURATION="$duration" \
        --env RAMP_UP="$ramp_up" \
        tests/performance/load.js
    
    log "✅ Teste de carga concluído: $output_file" "$GREEN"
}

# Função para teste de stress progressivo
stress_test() {
    log "💥 Executando teste de stress progressivo..." "$PURPLE"
    
    local users_list=(10 20 30 40 50 75 100)
    local duration=30
    local ramp_up=5
    
    for users in "${users_list[@]}"; do
        log "📊 Testando com $users usuários concorrentes..." "$YELLOW"
        
        local output_file="$RESULTS_DIR/stress_${users}users_$TIMESTAMP.json"
        
        k6 run \
            --out json="$output_file" \
            --env BASE_URL="$BASE_URL" \
            --env TEST_TYPE="stress" \
            --env USERS="$users" \
            --env DURATION="$duration" \
            --env RAMP_UP="$ramp_up" \
            tests/performance/stress.js
        
        # Aguardar entre testes
        sleep 5
    done
    
    log "✅ Teste de stress concluído" "$GREEN"
}

# Função para teste de spike
spike_test() {
    log "⚡ Executando teste de spike..." "$PURPLE"
    
    local output_file="$RESULTS_DIR/spike_$TIMESTAMP.json"
    
    k6 run \
        --out json="$output_file" \
        --env BASE_URL="$BASE_URL" \
        --env TEST_TYPE="spike" \
        tests/performance/spike.js
    
    log "✅ Teste de spike concluído: $output_file" "$GREEN"
}

# Função para teste de endurance
endurance_test() {
    local duration=${1:-300} # 5 minutos por padrão
    
    log "⏰ Executando teste de endurance (${duration}s)..." "$PURPLE"
    
    local output_file="$RESULTS_DIR/endurance_${duration}s_$TIMESTAMP.json"
    
    k6 run \
        --out json="$output_file" \
        --env BASE_URL="$BASE_URL" \
        --env TEST_TYPE="endurance" \
        --env USERS=20 \
        --env DURATION="$duration" \
        --env RAMP_UP=10 \
        tests/performance/endurance.js
    
    log "✅ Teste de endurance concluído: $output_file" "$GREEN"
}

# Função para teste específico de endpoint
endpoint_test() {
    local endpoint=$1
    local users=${2:-10}
    local duration=${3:-60}
    
    log "🎯 Testando endpoint específico: $endpoint" "$PURPLE"
    
    local output_file="$RESULTS_DIR/endpoint_${endpoint//\//_}_$TIMESTAMP.json"
    
    k6 run \
        --out json="$output_file" \
        --env BASE_URL="$BASE_URL" \
        --env TEST_TYPE="endpoint" \
        --env ENDPOINT="$endpoint" \
        --env USERS="$users" \
        --env DURATION="$duration" \
        tests/performance/endpoint.js
    
    log "✅ Teste de endpoint concluído: $output_file" "$GREEN"
}

# Função para análise de resultados
analyze_results() {
    log "📈 Analisando resultados..." "$CYAN"
    
    # Verificar se o diretório de resultados existe
    if [ ! -d "$RESULTS_DIR" ]; then
        log "❌ Diretório de resultados não encontrado" "$RED"
        return 1
    fi
    
    # Encontrar arquivos de resultado mais recentes
    local latest_files=$(find "$RESULTS_DIR" -name "*.json" -type f -newer "$RESULTS_DIR/.placeholder" 2>/dev/null || true)
    
    if [ -z "$latest_files" ]; then
        log "⚠️ Nenhum arquivo de resultado encontrado" "$YELLOW"
        return 1
    fi
    
    log "📊 Arquivos de resultado encontrados:" "$CYAN"
    echo "$latest_files" | while read -r file; do
        log "   • $(basename "$file")" "$CYAN"
    done
    
    # Gerar relatório resumido
    generate_report
}

# Função para gerar relatório
generate_report() {
    local report_file="$RESULTS_DIR/report_$TIMESTAMP.md"
    
    log "📋 Gerando relatório: $report_file" "$CYAN"
    
    cat > "$report_file" << EOF
# Relatório de Performance - Hackathon API

**Data/Hora**: $(date)
**Timestamp**: $TIMESTAMP

## Resumo Executivo

### Configurações de Teste
- **URL Base**: $BASE_URL
- **Ferramenta**: k6
- **Ambiente**: $(uname -s) $(uname -r)

### Resultados Principais

> **Nota**: Este é um relatório básico. Para análise detalhada, use os arquivos JSON gerados.

## Arquivos de Resultado

EOF

    # Listar arquivos de resultado
    find "$RESULTS_DIR" -name "*.json" -type f -newer "$RESULTS_DIR/.placeholder" 2>/dev/null | while read -r file; do
        echo "- \`$(basename "$file")\`" >> "$report_file"
    done

    cat >> "$report_file" << EOF

## Próximos Passos

1. Analisar arquivos JSON com ferramentas especializadas
2. Comparar com testes anteriores
3. Identificar gargalos de performance
4. Implementar otimizações necessárias

## Comandos Úteis

\`\`\`bash
# Executar teste específico
./scripts/performance-test.sh endpoint /simulacao 20 60

# Executar suite completa
./scripts/performance-test.sh full

# Analisar resultados
./scripts/performance-test.sh analyze
\`\`\`
EOF

    log "✅ Relatório gerado: $report_file" "$GREEN"
}

# Função para mostrar ajuda
show_help() {
    cat << EOF
🚀 API Performance Test Script

Uso: $0 [COMANDO] [OPÇÕES]

COMANDOS:
  health           Verificar saúde da API
  baseline         Teste de baseline (1 usuário, 30s)
  load [users]     Teste de carga (padrão: 50 usuários, 60s)
  stress           Teste de stress progressivo (10-100 usuários)
  spike            Teste de spike (picos de carga)
  endurance [s]    Teste de endurance (padrão: 300s)
  endpoint [path]  Teste específico de endpoint
  analyze          Analisar resultados
  full             Executar suite completa
  help             Mostrar esta ajuda

EXEMPLOS:
  $0 health
  $0 baseline
  $0 load 25
  $0 stress
  $0 endpoint /simulacao 20 60
  $0 endurance 600
  $0 full
  $0 analyze

CONFIGURAÇÕES:
  BASE_URL: $BASE_URL
  RESULTS_DIR: $RESULTS_DIR
  LOG_FILE: $LOG_FILE

REQUISITOS:
  - k6 instalado e configurado
  - API rodando em $BASE_URL
  - curl disponível
EOF
}

# Função principal
main() {
    # Criar diretório de resultados se não existir
    mkdir -p "$RESULTS_DIR"
    touch "$RESULTS_DIR/.placeholder"
    
    log "🚀 Iniciando testes de performance..." "$GREEN"
    log "📁 Resultados em: $RESULTS_DIR" "$CYAN"
    log "📝 Log em: $LOG_FILE" "$CYAN"
    
    case "${1:-help}" in
        "health")
            check_api_health
            ;;
        "baseline")
            check_api_health && baseline_test
            ;;
        "load")
            local users=${2:-50}
            local duration=${3:-60}
            check_api_health && load_test "$users" "$duration"
            ;;
        "stress")
            check_api_health && stress_test
            ;;
        "spike")
            check_api_health && spike_test
            ;;
        "endurance")
            local duration=${2:-300}
            check_api_health && endurance_test "$duration"
            ;;
        "endpoint")
            local endpoint=${2:-/simulacao}
            local users=${3:-10}
            local duration=${4:-60}
            check_api_health && endpoint_test "$endpoint" "$users" "$duration"
            ;;
        "analyze")
            analyze_results
            ;;
        "full")
            log "🔥 Executando suite completa de testes..." "$PURPLE"
            check_api_health || exit 1
            baseline_test
            load_test 50 60
            stress_test
            spike_test
            endurance_test 300
            analyze_results
            log "✅ Suite completa concluída!" "$GREEN"
            ;;
        "help"|*)
            show_help
            ;;
    esac
}

# Executar função principal
main "$@"
