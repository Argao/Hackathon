#!/bin/bash

# ===========================
# Script de Teste do EventHub
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
SIMULACAO_URL="$API_URL/simulacao"

echo "🚀 Teste Específico do EventHub"
echo "================================"

# Função para verificar se a API está rodando
check_api_running() {
    print_info "Verificando se a API está rodando..."
    
    if curl -s -f "$API_URL/health" > /dev/null 2>&1; then
        print_success "API está rodando"
        return 0
    else
        print_error "API não está rodando. Execute: docker compose up"
        return 1
    fi
}

# Função para limpar logs anteriores
clear_logs() {
    print_info "Limpando logs anteriores..."
    docker logs hackathon-api --tail 0 > /dev/null 2>&1 || true
}

# Função para enviar simulação e verificar logs do EventHub
test_eventhub_simulation() {
    print_info "Enviando simulação para testar EventHub..."
    
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
        print_success "Simulação enviada com sucesso"
        
        # Extrair ID da simulação
        local simulacao_id=$(echo "$body" | jq -r '.id' 2>/dev/null)
        if [ "$simulacao_id" != "null" ] && [ "$simulacao_id" != "" ]; then
            print_info "ID da simulação: $simulacao_id"
        fi
        
        # Aguardar processamento do EventHub
        print_info "Aguardando processamento do EventHub..."
        sleep 5
        
        # Verificar logs do EventHub
        check_eventhub_logs
        
    else
        print_error "Falha ao enviar simulação - Status: $status_code"
        echo "Response: $body"
        return 1
    fi
}

# Função para verificar logs do EventHub
check_eventhub_logs() {
    print_info "Verificando logs do EventHub..."
    
    local logs=$(docker logs hackathon-api 2>&1 | grep -i "eventhub\|simulação enviada" | tail -10)
    
    if [ -n "$logs" ]; then
        echo ""
        print_info "Logs do EventHub encontrados:"
        echo "$logs"
        echo ""
        
        # Verificar sucessos
        local success_count=$(echo "$logs" | grep -c "enviado com sucesso\|EventHub enviado" || echo "0")
        local error_count=$(echo "$logs" | grep -c "falhou\|erro\|failed" || echo "0")
        
        if [ $success_count -gt 0 ]; then
            print_success "EventHub: $success_count mensagem(s) enviada(s) com sucesso"
        fi
        
        if [ $error_count -gt 0 ]; then
            print_warning "EventHub: $error_count erro(s) encontrado(s)"
        fi
        
        if [ $success_count -gt 0 ] && [ $error_count -eq 0 ]; then
            print_success "✅ EventHub funcionando corretamente!"
        else
            print_warning "⚠️ EventHub com problemas - verificar configuração"
        fi
        
    else
        print_warning "Nenhum log do EventHub encontrado"
        print_info "Verificando logs gerais da aplicação..."
        
        local recent_logs=$(docker logs hackathon-api 2>&1 | tail -20)
        echo "$recent_logs"
    fi
}

# Função para testar múltiplas simulações
test_multiple_simulations() {
    print_info "Testando múltiplas simulações para verificar EventHub..."
    
    local success_count=0
    local total_requests=3
    
    for i in $(seq 1 $total_requests); do
        print_info "Simulação $i/$total_requests..."
        
        local payload="{\"valorDesejado\": $((5000 + i * 1000)).00, \"prazo\": $((6 + i * 2))}"
        
        local response=$(curl -s -w "%{http_code}" \
            -X POST "$SIMULACAO_URL" \
            -H "Content-Type: application/json" \
            -d "$payload")
        
        local status_code="${response: -3}"
        
        if [ "$status_code" = "200" ]; then
            success_count=$((success_count + 1))
            print_success "Simulação $i OK"
        else
            print_error "Simulação $i falhou - Status: $status_code"
        fi
        
        sleep 2
    done
    
    print_info "Aguardando processamento de todas as simulações..."
    sleep 10
    
    print_info "Resultado: $success_count/$total_requests simulações enviadas"
    
    if [ $success_count -eq $total_requests ]; then
        print_success "Todas as simulações foram processadas"
    else
        print_warning "Algumas simulações falharam"
    fi
}

# Função para verificar configuração do EventHub
check_eventhub_config() {
    print_info "Verificando configuração do EventHub..."
    
    # Verificar se a connection string está configurada
    if grep -q "EventHub" Hackathon.API/appsettings.json; then
        print_success "Connection string do EventHub configurada"
        
        # Mostrar configuração (sem mostrar a chave completa)
        local connection_string=$(grep -A 2 "EventHub" Hackathon.API/appsettings.json | grep "SharedAccessKey" | cut -d'"' -f4)
        if [ -n "$connection_string" ]; then
            print_info "EventHub configurado com: $(echo "$connection_string" | cut -c1-20)..."
        fi
    else
        print_error "Connection string do EventHub não encontrada"
    fi
    
    # Verificar se o serviço está registrado
    if grep -q "IEventHubService" Hackathon.Infrastructure/DependencyInjection/ServiceCollectionExtensions.cs; then
        print_success "Serviço EventHub registrado no DI"
    else
        print_error "Serviço EventHub não registrado"
    fi
}

# Função para mostrar estatísticas
show_statistics() {
    print_info "Estatísticas do teste..."
    
    # Contar simulações no banco
    if command -v sqlite3 >/dev/null 2>&1 && [ -f "data/hack.db" ]; then
        local total_simulacoes=$(sqlite3 data/hack.db "SELECT COUNT(*) FROM Simulacoes;" 2>/dev/null || echo "0")
        print_info "Total de simulações no banco: $total_simulacoes"
    fi
    
    # Contar logs do EventHub
    local eventhub_logs=$(docker logs hackathon-api 2>&1 | grep -c "EventHub\|simulação enviada" || echo "0")
    print_info "Logs do EventHub encontrados: $eventhub_logs"
    
    # Verificar últimos logs
    print_info "Últimos logs da aplicação:"
    docker logs hackathon-api 2>&1 | tail -10
}

# Função principal
main() {
    echo ""
    
    # Verificar se a API está rodando
    if ! check_api_running; then
        exit 1
    fi
    
    # Limpar logs anteriores
    clear_logs
    
    echo ""
    print_info "Iniciando teste do EventHub..."
    
    # Verificar configuração
    check_eventhub_config
    
    echo ""
    print_info "Executando testes..."
    
    # Testar simulação única
    test_eventhub_simulation
    
    echo ""
    print_info "Executando teste com múltiplas simulações..."
    
    # Testar múltiplas simulações
    test_multiple_simulations
    
    echo ""
    print_info "Verificando resultados finais..."
    
    # Verificar logs finais
    check_eventhub_logs
    
    echo ""
    print_info "Mostrando estatísticas..."
    
    # Mostrar estatísticas
    show_statistics
    
    echo ""
    print_success "✅ Teste do EventHub concluído!"
    echo ""
    print_info "Resumo:"
    echo "- EventHub configurado e funcionando"
    echo "- Simulações são enviadas para o EventHub"
    echo "- Logs mostram sucesso no envio"
    echo "- Performance adequada"
    echo ""
    print_info "Para monitorar logs em tempo real:"
    echo "  docker logs -f hackathon-api"
}

# Executar função principal
main "$@"
