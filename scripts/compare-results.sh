#!/bin/bash

# ============================================================
# 📊 COMPARE PERFORMANCE RESULTS
#   Compara resultados de testes de performance
# ============================================================

set -e

# Configurações
RESULTS_DIR="./test-results"
COMPARISON_DIR="./test-results/comparisons"

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
    echo -e "${2:-$BLUE}$1${NC}"
}

# Função para extrair métricas de arquivo JSON
extract_metrics() {
    local file=$1
    
    if [ ! -f "$file" ]; then
        log "❌ Arquivo não encontrado: $file" "$RED"
        return 1
    fi
    
    # Extrair métricas usando jq (se disponível) ou grep
    if command -v jq &> /dev/null; then
        local metrics=$(jq -r '
            {
                "total_requests": .metrics.http_reqs.values.count,
                "failed_requests": .metrics.http_req_failed.values.rate * .metrics.http_reqs.values.count,
                "avg_response_time": .metrics.http_req_duration.values.avg,
                "p95_response_time": .metrics.http_req_duration.values["p(95)"],
                "p99_response_time": .metrics.http_req_duration.values["p(99)"],
                "min_response_time": .metrics.http_req_duration.values.min,
                "max_response_time": .metrics.http_req_duration.values.max,
                "rps": .metrics.http_reqs.values.rate,
                "error_rate": .metrics.http_req_failed.values.rate * 100
            } | to_entries[] | "\(.key): \(.value)"
        ' "$file" 2>/dev/null)
        
        if [ $? -eq 0 ]; then
            echo "$metrics"
        else
            log "⚠️ Erro ao processar JSON com jq, usando método alternativo" "$YELLOW"
            extract_metrics_simple "$file"
        fi
    else
        extract_metrics_simple "$file"
    fi
}

# Função para extrair métricas usando grep (fallback)
extract_metrics_simple() {
    local file=$1
    
    # Extrair métricas básicas usando grep
    local total_requests=$(grep -o '"count":[0-9]*' "$file" | head -1 | cut -d: -f2)
    local avg_response_time=$(grep -o '"avg":[0-9.]*' "$file" | head -1 | cut -d: -f2)
    local p95_response_time=$(grep -o '"p\\(95\\)":[0-9.]*' "$file" | head -1 | cut -d: -f2)
    local rps=$(grep -o '"rate":[0-9.]*' "$file" | head -1 | cut -d: -f2)
    
    echo "total_requests: $total_requests"
    echo "avg_response_time: $avg_response_time"
    echo "p95_response_time: $p95_response_time"
    echo "rps: $rps"
}

# Função para comparar dois arquivos
compare_files() {
    local file1=$1
    local file2=$2
    local output_file=$3
    
    log "📊 Comparando resultados..." "$CYAN"
    log "   📁 Arquivo 1: $(basename "$file1")" "$CYAN"
    log "   📁 Arquivo 2: $(basename "$file2")" "$CYAN"
    
    # Extrair métricas
    local metrics1=$(extract_metrics "$file1")
    local metrics2=$(extract_metrics "$file2")
    
    if [ -z "$metrics1" ] || [ -z "$metrics2" ]; then
        log "❌ Erro ao extrair métricas dos arquivos" "$RED"
        return 1
    fi
    
    # Criar relatório de comparação
    cat > "$output_file" << EOF
# 📊 Comparação de Performance - Hackathon API

**Data/Hora**: $(date)
**Arquivo 1**: $(basename "$file1")
**Arquivo 2**: $(basename "$file2")

## 📈 Métricas Comparativas

| Métrica | Arquivo 1 | Arquivo 2 | Diferença | Status |
|---------|-----------|-----------|-----------|--------|
EOF

    # Comparar métricas
    while IFS= read -r line; do
        local metric=$(echo "$line" | cut -d: -f1)
        local value1=$(echo "$line" | cut -d: -f2 | xargs)
        
        # Encontrar valor correspondente no arquivo 2
        local value2=$(echo "$metrics2" | grep "^$metric:" | cut -d: -f2 | xargs)
        
        if [ -n "$value1" ] && [ -n "$value2" ]; then
            # Calcular diferença percentual
            local diff_percent=0
            local status="🟡"
            
            if [ "$value1" != "0" ] && [ "$value2" != "0" ]; then
                diff_percent=$(echo "scale=2; (($value2 - $value1) / $value1) * 100" | bc 2>/dev/null || echo "0")
                
                # Determinar status baseado na métrica
                case $metric in
                    "rps"|"total_requests")
                        if (( $(echo "$diff_percent > 0" | bc -l) )); then
                            status="🟢"
                        else
                            status="🔴"
                        fi
                        ;;
                    "avg_response_time"|"p95_response_time"|"p99_response_time"|"max_response_time"|"error_rate")
                        if (( $(echo "$diff_percent < 0" | bc -l) )); then
                            status="🟢"
                        else
                            status="🔴"
                        fi
                        ;;
                esac
            fi
            
            echo "| $metric | $value1 | $value2 | ${diff_percent}% | $status |" >> "$output_file"
        fi
    done <<< "$metrics1"
    
    cat >> "$output_file" << EOF

## 📋 Análise

### 🟢 Melhorias
- Métricas que melhoraram entre os testes

### 🔴 Degradações  
- Métricas que pioraram entre os testes

### 🟡 Estáveis
- Métricas que permaneceram similares

## 💡 Recomendações

Baseado na comparação, considere:

1. **Otimizações necessárias** para métricas que pioraram
2. **Manutenção** das otimizações que funcionaram
3. **Monitoramento contínuo** das métricas críticas

---
**Gerado automaticamente** em $(date)
EOF

    log "✅ Relatório de comparação gerado: $output_file" "$GREEN"
}

# Função para listar arquivos disponíveis
list_available_files() {
    log "📁 Arquivos de resultado disponíveis:" "$CYAN"
    
    local files=($(find "$RESULTS_DIR" -name "*.json" -type f | sort))
    
    if [ ${#files[@]} -eq 0 ]; then
        log "❌ Nenhum arquivo de resultado encontrado em $RESULTS_DIR" "$RED"
        return 1
    fi
    
    for i in "${!files[@]}"; do
        local file="${files[$i]}"
        local basename_file=$(basename "$file")
        local timestamp=$(echo "$basename_file" | grep -o '[0-9]\{8\}_[0-9]\{6\}' || echo "N/A")
        
        log "   $((i+1)). $basename_file (${timestamp})" "$CYAN"
    done
    
    echo "${files[@]}"
}

# Função para seleção interativa
select_files() {
    local files=($(list_available_files))
    
    if [ $? -ne 0 ]; then
        return 1
    fi
    
    log "🎯 Selecione dois arquivos para comparar:" "$PURPLE"
    
    # Selecionar primeiro arquivo
    local file1=""
    while [ -z "$file1" ]; do
        read -p "Digite o número do primeiro arquivo: " choice1
        if [[ "$choice1" =~ ^[0-9]+$ ]] && [ "$choice1" -ge 1 ] && [ "$choice1" -le ${#files[@]} ]; then
            file1="${files[$((choice1-1))]}"
        else
            log "❌ Escolha inválida. Digite um número entre 1 e ${#files[@]}" "$RED"
        fi
    done
    
    # Selecionar segundo arquivo
    local file2=""
    while [ -z "$file2" ]; do
        read -p "Digite o número do segundo arquivo: " choice2
        if [[ "$choice2" =~ ^[0-9]+$ ]] && [ "$choice2" -ge 1 ] && [ "$choice2" -le ${#files[@]} ]; then
            if [ "$choice2" -eq "$choice1" ]; then
                log "❌ Escolha o mesmo arquivo. Selecione um arquivo diferente." "$RED"
            else
                file2="${files[$((choice2-1))]}"
            fi
        else
            log "❌ Escolha inválida. Digite um número entre 1 e ${#files[@]}" "$RED"
        fi
    done
    
    echo "$file1 $file2"
}

# Função para mostrar ajuda
show_help() {
    cat << EOF
📊 Compare Performance Results

Uso: $0 [OPÇÕES]

OPÇÕES:
  -f, --file1 FILE     Primeiro arquivo para comparação
  -s, --file2 FILE     Segundo arquivo para comparação
  -o, --output FILE    Arquivo de saída (padrão: comparison_[timestamp].md)
  -i, --interactive    Modo interativo para seleção de arquivos
  -l, --list           Listar arquivos disponíveis
  -h, --help           Mostrar esta ajuda

EXEMPLOS:
  $0 -f test-results/baseline_20240115_143022.json -s test-results/baseline_20240115_150000.json
  $0 -i
  $0 -l

REQUISITOS:
  - Arquivos JSON de resultado do k6
  - jq (opcional, para melhor parsing)
  - bc (opcional, para cálculos)
EOF
}

# Função principal
main() {
    # Criar diretório de comparações se não existir
    mkdir -p "$COMPARISON_DIR"
    
    local file1=""
    local file2=""
    local output_file=""
    local interactive=false
    local list_files=false
    
    # Parse argumentos
    while [[ $# -gt 0 ]]; do
        case $1 in
            -f|--file1)
                file1="$2"
                shift 2
                ;;
            -s|--file2)
                file2="$2"
                shift 2
                ;;
            -o|--output)
                output_file="$2"
                shift 2
                ;;
            -i|--interactive)
                interactive=true
                shift
                ;;
            -l|--list)
                list_files=true
                shift
                ;;
            -h|--help)
                show_help
                exit 0
                ;;
            *)
                log "❌ Opção desconhecida: $1" "$RED"
                show_help
                exit 1
                ;;
        esac
    done
    
    # Ações baseadas nos argumentos
    if [ "$list_files" = true ]; then
        list_available_files
        exit 0
    fi
    
    if [ "$interactive" = true ]; then
        local files=$(select_files)
        if [ $? -eq 0 ]; then
            read -r file1 file2 <<< "$files"
        else
            exit 1
        fi
    fi
    
    # Verificar se temos os arquivos necessários
    if [ -z "$file1" ] || [ -z "$file2" ]; then
        log "❌ Arquivos de entrada não especificados" "$RED"
        show_help
        exit 1
    fi
    
    # Gerar nome do arquivo de saída se não especificado
    if [ -z "$output_file" ]; then
        local timestamp=$(date +"%Y%m%d_%H%M%S")
        output_file="$COMPARISON_DIR/comparison_${timestamp}.md"
    fi
    
    # Executar comparação
    compare_files "$file1" "$file2" "$output_file"
    
    log "🎉 Comparação concluída!" "$GREEN"
    log "📄 Relatório: $output_file" "$CYAN"
}

# Executar função principal
main "$@"
