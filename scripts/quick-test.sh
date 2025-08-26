#!/bin/bash

# ===========================
# Teste Rápido do Sistema
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

echo "⚡ Teste Rápido do Sistema"
echo "=========================="

# Verificar se Docker está rodando
if ! docker info > /dev/null 2>&1; then
    print_error "Docker não está rodando"
    exit 1
fi

print_success "Docker está rodando"

# Verificar se os containers estão ativos
if docker ps | grep -q "hackathon-api"; then
    print_success "Container da API está rodando"
else
    print_warning "Container da API não está rodando"
    print_info "Execute: docker compose up -d"
    exit 1
fi

# Testar endpoint de health
API_URL="http://localhost:8080"
if curl -s -f "$API_URL/health" > /dev/null 2>&1; then
    print_success "API está respondendo"
else
    print_error "API não está respondendo"
    exit 1
fi

# Testar simulação rápida
print_info "Testando simulação..."
RESPONSE=$(curl -s -X POST "$API_URL/simulacao" \
    -H "Content-Type: application/json" \
    -d '{"valorDesejado": 10000.00, "prazo": 12}')

if echo "$RESPONSE" | jq -e '.idSimulacao' > /dev/null 2>&1; then
    print_success "Simulação funcionando"
    SIM_ID=$(echo "$RESPONSE" | jq -r '.idSimulacao')
    print_info "ID da simulação: $SIM_ID"
else
    print_error "Simulação falhou"
    echo "Response: $RESPONSE"
    exit 1
fi

# Verificar logs do EventHub
sleep 3
if docker logs hackathon-api 2>&1 | grep -q "Simulação enviada com sucesso para o EventHub"; then
    print_success "EventHub funcionando"
else
    print_warning "EventHub pode ter problemas - verificar logs"
fi

# Verificar banco de dados
if [ -f "data/hack.db" ]; then
    print_success "Banco de dados existe"
    if command -v sqlite3 >/dev/null 2>&1; then
        COUNT=$(sqlite3 data/hack.db "SELECT COUNT(*) FROM Simulacoes;" 2>/dev/null || echo "0")
        print_info "Simulações no banco: $COUNT"
    fi
else
    print_warning "Banco de dados não existe ainda"
fi

echo ""
print_success "✅ Sistema funcionando corretamente!"
echo ""
print_info "Para testes mais detalhados:"
echo "  ./scripts/test-integration.sh"
echo "  ./scripts/test-eventhub.sh"
echo ""
print_info "Para acessar Swagger:"
echo "  $API_URL/swagger"
