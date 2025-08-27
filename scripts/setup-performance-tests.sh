#!/bin/bash

# ============================================================
# 🚀 SETUP PERFORMANCE TESTS
#   Configuração completa para testes de performance
# ============================================================

set -e

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

# Função para verificar se comando existe
command_exists() {
    command -v "$1" >/dev/null 2>&1
}

# Função para instalar dependências
install_dependencies() {
    log "📦 Instalando dependências..." "$CYAN"
    
    local missing_deps=()
    
    # Verificar k6
    if ! command_exists k6; then
        missing_deps+=("k6")
    fi
    
    # Verificar curl
    if ! command_exists curl; then
        missing_deps+=("curl")
    fi
    
    # Verificar jq (opcional)
    if ! command_exists jq; then
        log "⚠️ jq não encontrado (opcional, para melhor análise)" "$YELLOW"
    fi
    
    # Verificar bc (opcional)
    if ! command_exists bc; then
        log "⚠️ bc não encontrado (opcional, para cálculos)" "$YELLOW"
    fi
    
    # Instalar dependências faltantes
    if [ ${#missing_deps[@]} -gt 0 ]; then
        log "🔧 Instalando dependências faltantes..." "$YELLOW"
        
        if command_exists apt-get; then
            sudo apt-get update
            sudo apt-get install -y "${missing_deps[@]}"
        elif command_exists yum; then
            sudo yum install -y "${missing_deps[@]}"
        elif command_exists dnf; then
            sudo dnf install -y "${missing_deps[@]}"
        elif command_exists brew; then
            brew install "${missing_deps[@]}"
        else
            log "❌ Gerenciador de pacotes não suportado" "$RED"
            log "🔧 Instale manualmente: ${missing_deps[*]}" "$YELLOW"
            return 1
        fi
    fi
    
    log "✅ Dependências verificadas" "$GREEN"
}

# Função para criar estrutura de diretórios
create_directories() {
    log "📁 Criando estrutura de diretórios..." "$CYAN"
    
    # Diretórios principais
    mkdir -p test-results
    mkdir -p test-results/comparisons
    mkdir -p tests/performance
    
    # Verificar se os scripts k6 existem
    local k6_scripts=(
        "tests/performance/baseline.js"
        "tests/performance/load.js"
        "tests/performance/stress.js"
        "tests/performance/spike.js"
        "tests/performance/endurance.js"
        "tests/performance/endpoint.js"
    )
    
    local missing_scripts=()
    for script in "${k6_scripts[@]}"; do
        if [ ! -f "$script" ]; then
            missing_scripts+=("$script")
        fi
    done
    
    if [ ${#missing_scripts[@]} -gt 0 ]; then
        log "❌ Scripts k6 faltando: ${missing_scripts[*]}" "$RED"
        return 1
    fi
    
    log "✅ Estrutura de diretórios criada" "$GREEN"
}

# Função para verificar permissões
check_permissions() {
    log "🔐 Verificando permissões..." "$CYAN"
    
    local scripts=(
        "scripts/performance-test.sh"
        "scripts/install-k6.sh"
        "scripts/compare-results.sh"
        "scripts/setup-performance-tests.sh"
    )
    
    for script in "${scripts[@]}"; do
        if [ -f "$script" ] && [ ! -x "$script" ]; then
            log "🔧 Tornando executável: $script" "$YELLOW"
            chmod +x "$script"
        fi
    done
    
    log "✅ Permissões verificadas" "$GREEN"
}

# Função para verificar configuração da API
check_api_config() {
    log "🔍 Verificando configuração da API..." "$CYAN"
    
    local api_url="http://localhost:8080"
    
    # Verificar se a API está respondendo
    if curl -s -f "$api_url/health" > /dev/null 2>&1; then
        log "✅ API está respondendo em $api_url" "$GREEN"
    else
        log "⚠️ API não está respondendo em $api_url" "$YELLOW"
        log "🔧 Certifique-se de que a API está rodando antes de executar os testes" "$YELLOW"
    fi
}

# Função para executar teste de validação
run_validation_test() {
    log "🧪 Executando teste de validação..." "$CYAN"
    
    # Teste simples de baseline
    if ./scripts/performance-test.sh baseline > /dev/null 2>&1; then
        log "✅ Teste de validação passou" "$GREEN"
    else
        log "❌ Teste de validação falhou" "$RED"
        log "🔧 Verifique se a API está rodando e acessível" "$YELLOW"
        return 1
    fi
}

# Função para mostrar resumo
show_summary() {
    log "📋 Resumo da configuração:" "$PURPLE"
    echo
    log "✅ Dependências instaladas" "$GREEN"
    log "✅ Estrutura de diretórios criada" "$GREEN"
    log "✅ Permissões configuradas" "$GREEN"
    log "✅ Scripts k6 disponíveis" "$GREEN"
    echo
    log "🚀 Próximos passos:" "$CYAN"
    echo
    log "1. Iniciar a API:" "$YELLOW"
    log "   dotnet run --project Hackathon.API" "$CYAN"
    echo
    log "2. Executar teste de baseline:" "$YELLOW"
    log "   ./scripts/performance-test.sh baseline" "$CYAN"
    echo
    log "3. Executar suite completa:" "$YELLOW"
    log "   ./scripts/performance-test.sh full" "$CYAN"
    echo
    log "4. Comparar resultados:" "$YELLOW"
    log "   ./scripts/compare-results.sh -i" "$CYAN"
    echo
    log "5. Ver ajuda completa:" "$YELLOW"
    log "   ./scripts/performance-test.sh help" "$CYAN"
    echo
    log "📚 Documentação:" "$CYAN"
    log "   tests/performance/README.md" "$CYAN"
}

# Função para mostrar ajuda
show_help() {
    cat << EOF
🚀 Setup Performance Tests

Uso: $0 [OPÇÕES]

OPÇÕES:
  -f, --force          Forçar reinstalação de dependências
  -v, --validate       Executar teste de validação
  -s, --skip-deps      Pular instalação de dependências
  -h, --help           Mostrar esta ajuda

EXEMPLOS:
  $0                    # Setup completo
  $0 -v                 # Setup + validação
  $0 -f                 # Setup forçado
  $0 -s                 # Setup sem dependências

REQUISITOS:
  - Sistema operacional suportado (Linux, macOS, Windows)
  - Acesso sudo (para instalação de dependências)
  - API rodando em http://localhost:8080
EOF
}

# Função principal
main() {
    local force=false
    local validate=false
    local skip_deps=false
    
    # Parse argumentos
    while [[ $# -gt 0 ]]; do
        case $1 in
            -f|--force)
                force=true
                shift
                ;;
            -v|--validate)
                validate=true
                shift
                ;;
            -s|--skip-deps)
                skip_deps=true
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
    
    log "🚀 Iniciando setup dos testes de performance..." "$GREEN"
    echo
    
    # Instalar dependências (se não pulado)
    if [ "$skip_deps" = false ]; then
        install_dependencies
        echo
    fi
    
    # Criar estrutura de diretórios
    create_directories
    echo
    
    # Verificar permissões
    check_permissions
    echo
    
    # Verificar configuração da API
    check_api_config
    echo
    
    # Executar teste de validação (se solicitado)
    if [ "$validate" = true ]; then
        run_validation_test
        echo
    fi
    
    # Mostrar resumo
    show_summary
    
    log "🎉 Setup concluído com sucesso!" "$GREEN"
}

# Executar função principal
main "$@"
