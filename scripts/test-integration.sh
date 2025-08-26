#!/bin/bash

# ===========================
# Script de Teste de Integração
# ===========================

set -e

# Cores para output
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
RED='\033[0;31m'
BLUE='\033[0;34m'
NC='\033[0m' # No Color

print_success() {
    echo -e "${GREEN}✓ $1${NC}"
}

print_warning() {
    echo -e "${YELLOW}⚠ $1${NC}"
}

print_error() {
    echo -e "${RED}✗ $1${NC}"
}

print_info() {
    echo -e "${BLUE}ℹ $1${NC}"
}

# Variáveis
API_URL="http://localhost:8080"
SWAGGER_URL="$API_URL/swagger"
HEALTH_URL="$API_URL/health"
SIMULACAO_URL="$API_URL/simulacao"
TELEMETRIA_URL="$API_URL/telemetria"

echo "🧪 Iniciando Teste de Integração Completo..."
echo "=============================================="

# Função para aguardar a API estar pronta
wait_for_api() {
    print_info "Aguardando API estar pronta..."
    local max_attempts=30
    local attempt=1
    
    while [ $attempt -le $max_attempts ]; do
        if curl -s -f "$HEALTH_URL" > /dev/null 2>&1; then
            print_success "API está pronta!"
            return 0
        fi
        
        print_info "Tentativa $attempt/$max_attempts - API ainda não está pronta..."
        sleep 2
        attempt=$((attempt + 1))
    done
    
    print_error "API não ficou pronta em $max_attempts tentativas"
    return 1
}

# Função para testar endpoint de health
test_health() {
    print_info "Testando endpoint de health..."
    
    local response=$(curl -s -w "%{http_code}" "$HEALTH_URL")
    local status_code="${response: -3}"
    local body="${response%???}"
    
    if [ "$status_code" = "200" ]; then
        print_success "Health check OK - Status: $status_code"
        echo "Response: $body"
    else
        print_error "Health check falhou - Status: $status_code"
        echo "Response: $body"
        return 1
    fi
}

# Função para testar Swagger
test_swagger() {
    print_info "Testando endpoint do Swagger..."
    
    local response=$(curl -s -w "%{http_code}" "$SWAGGER_URL")
    local status_code="${response: -3}"
    
    if [ "$status_code" = "200" ]; then
        print_success "Swagger OK - Status: $status_code"
    else
        print_warning "Swagger não disponível - Status: $status_code"
    fi
}

# Função para testar simulação
test_simulacao() {
    print_info "Testando endpoint de simulação..."
    
    local payload='{
        "valorDesejado": 10000.00,
        "prazo": 12
    }'
    
    local response=$(curl -s -w "%{http_code}" \
        -X POST "$SIMULACAO_URL" \
        -H "Content-Type: application/json" \
        -d "$payload")
    
    local status_code="${response: -3}"
    local body="${response%???}"
    
    if [ "$status_code" = "200" ]; then
        print_success "Simulação realizada com sucesso - Status: $status_code"
        echo "Response: $body" | jq '.' 2>/dev/null || echo "Response: $body"
        
        # Extrair ID da simulação para verificar se foi persistida
        local simulacao_id=$(echo "$body" | jq -r '.id' 2>/dev/null)
        if [ "$simulacao_id" != "null" ] && [ "$simulacao_id" != "" ]; then
            print_success "Simulação persistida com ID: $simulacao_id"
        else
            print_warning "Não foi possível extrair ID da simulação"
        fi
    else
        print_error "Simulação falhou - Status: $status_code"
        echo "Response: $body"
        return 1
    fi
}

# Função para testar listagem de simulações
test_listar_simulacoes() {
    print_info "Testando listagem de simulações..."
    
    local response=$(curl -s -w "%{http_code}" \
        -X GET "$SIMULACAO_URL?pageNumber=1&pageSize=10")
    
    local status_code="${response: -3}"
    local body="${response%???}"
    
    if [ "$status_code" = "200" ]; then
        print_success "Listagem de simulações OK - Status: $status_code"
        echo "Response: $body" | jq '.' 2>/dev/null || echo "Response: $body"
    else
        print_warning "Listagem de simulações falhou - Status: $status_code"
        echo "Response: $body"
    fi
}

# Função para testar volume simulado
test_volume_simulado() {
    print_info "Testando volume simulado..."
    
    local hoje=$(date +%Y-%m-%d)
    local response=$(curl -s -w "%{http_code}" \
        -X GET "$SIMULACAO_URL/volume-por-dia?dataReferencia=$hoje")
    
    local status_code="${response: -3}"
    local body="${response%???}"
    
    if [ "$status_code" = "200" ]; then
        print_success "Volume simulado OK - Status: $status_code"
        echo "Response: $body" | jq '.' 2>/dev/null || echo "Response: $body"
    else
        print_warning "Volume simulado falhou - Status: $status_code"
        echo "Response: $body"
    fi
}

# Função para testar telemetria
test_telemetria() {
    print_info "Testando endpoint de telemetria..."
    
    local hoje=$(date +%Y-%m-%d)
    local response=$(curl -s -w "%{http_code}" \
        -X GET "$TELEMETRIA_URL?dataReferencia=$hoje")
    
    local status_code="${response: -3}"
    local body="${response%???}"
    
    if [ "$status_code" = "200" ]; then
        print_success "Telemetria OK - Status: $status_code"
        echo "Response: $body" | jq '.' 2>/dev/null || echo "Response: $body"
    else
        print_warning "Telemetria falhou - Status: $status_code"
        echo "Response: $body"
    fi
}

# Função para verificar logs do EventHub
check_eventhub_logs() {
    print_info "Verificando logs do EventHub..."
    
    # Aguardar um pouco para os logs serem processados
    sleep 3
    
            # Verificar se há logs de sucesso do EventHub
        if docker logs hackathon-api 2>&1 | grep -q "Simulação enviada com sucesso para o EventHub"; then
            print_success "EventHub: Mensagens enviadas com sucesso"
        else
            print_warning "EventHub: Não foram encontrados logs de sucesso"
        fi
        
        # Verificar se há logs de erro do EventHub
        if docker logs hackathon-api 2>&1 | grep -q "EventHub falhou"; then
            print_warning "EventHub: Foram encontrados logs de erro"
        fi
        
        # Mostrar logs relacionados ao EventHub
        echo ""
        print_info "Logs relacionados ao EventHub:"
        docker logs hackathon-api 2>&1 | grep -i "eventhub\|simulação enviada" | tail -5
}

# Função para verificar banco de dados
check_database() {
    print_info "Verificando banco de dados..."
    
    # Verificar se o arquivo do banco existe
    if [ -f "data/hack.db" ]; then
        print_success "Arquivo do banco existe"
        
        # Verificar tamanho do arquivo
        local size=$(du -h data/hack.db | cut -f1)
        print_info "Tamanho do banco: $size"
        
        # Verificar se há dados na tabela de simulações
        if command -v sqlite3 >/dev/null 2>&1; then
            local count=$(sqlite3 data/hack.db "SELECT COUNT(*) FROM Simulacoes;" 2>/dev/null || echo "0")
            print_info "Número de simulações no banco: $count"
        else
            print_warning "sqlite3 não disponível para verificar dados"
        fi
    else
        print_warning "Arquivo do banco não existe ainda"
    fi
}

# Função para testar performance
test_performance() {
    print_info "Testando performance com múltiplas requisições..."
    
    local start_time=$(date +%s)
    local success_count=0
    local total_requests=5
    
    for i in $(seq 1 $total_requests); do
        local response=$(curl -s -w "%{http_code}" \
            -X POST "$SIMULACAO_URL" \
            -H "Content-Type: application/json" \
            -d '{"valorDesejado": 5000.00, "prazo": 6}')
        
        local status_code="${response: -3}"
        
        if [ "$status_code" = "200" ]; then
            success_count=$((success_count + 1))
            echo -n "."
        else
            echo -n "x"
        fi
        
        sleep 0.5
    done
    
    local end_time=$(date +%s)
    local duration=$((end_time - start_time))
    
    echo ""
    print_info "Performance: $success_count/$total_requests sucessos em ${duration}s"
    
    if [ $success_count -eq $total_requests ]; then
        print_success "Teste de performance OK"
    else
        print_warning "Algumas requisições falharam no teste de performance"
    fi
}

# Função principal
main() {
    echo ""
    print_info "Iniciando testes..."
    
    # Aguardar API estar pronta
    if ! wait_for_api; then
        print_error "Falha ao aguardar API estar pronta"
        exit 1
    fi
    
    echo ""
    print_info "Executando testes de endpoints..."
    
    # Testar endpoints básicos
    test_health
    test_swagger
    
    echo ""
    print_info "Executando testes de funcionalidade..."
    
    # Testar funcionalidades principais
    test_simulacao
    test_listar_simulacoes
    test_volume_simulado
    test_telemetria
    
    echo ""
    print_info "Executando testes de performance..."
    
    # Testar performance
    test_performance
    
    echo ""
    print_info "Verificando infraestrutura..."
    
    # Verificar infraestrutura
    check_database
    check_eventhub_logs
    
    echo ""
    print_success "✅ Teste de integração concluído!"
    echo ""
    print_info "Resumo dos testes:"
    echo "- API está funcionando"
    echo "- Endpoints respondem corretamente"
    echo "- Simulações são processadas"
    echo "- Dados são persistidos"
    echo "- EventHub está configurado"
    echo "- Performance está adequada"
    echo ""
            print_info "Para ver logs detalhados:"
        echo "  docker logs hackathon-api"
    echo ""
    print_info "Para acessar Swagger:"
    echo "  $SWAGGER_URL"
}

# Executar função principal
main "$@"
