#!/bin/bash

# ===========================
# Script de Teste do SQLite
# ===========================

set -e

# Cores para output
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
RED='\033[0;31m'
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

echo "🧪 Testando configuração do SQLite..."

# Verificar se a pasta data existe
if [ -d "data" ]; then
    print_success "Pasta data existe"
else
    print_error "Pasta data não existe"
    exit 1
fi

# Verificar permissões da pasta data
if [ -w "data" ]; then
    print_success "Permissões de escrita OK na pasta data"
else
    print_error "Sem permissão de escrita na pasta data"
    exit 1
fi

# Verificar se o arquivo .gitkeep existe
if [ -f "data/.gitkeep" ]; then
    print_success "Arquivo .gitkeep existe"
else
    print_warning "Arquivo .gitkeep não existe"
fi

# Verificar se o arquivo hack.db existe (pode não existir ainda)
if [ -f "data/hack.db" ]; then
    print_success "Arquivo hack.db existe"
    ls -la data/hack.db
else
    print_warning "Arquivo hack.db não existe ainda (será criado na primeira execução)"
fi

# Verificar configurações do Docker
echo ""
echo "🐳 Verificando configurações do Docker..."

# Verificar se o .dockerignore não exclui a pasta data
if grep -q "^\*\*/data/$" .dockerignore; then
    print_error "Pasta data está sendo excluída no .dockerignore"
else
    print_success "Pasta data não está sendo excluída no .dockerignore"
fi

# Verificar se arquivos de banco estão sendo ignorados corretamente
if grep -q "data/\*\.db" .gitignore; then
    print_success "Arquivos de banco na pasta data estão sendo ignorados no .gitignore"
else
    print_warning "Arquivos de banco na pasta data podem não estar sendo ignorados"
fi

# Verificar se o docker-compose está configurado corretamente
if grep -q "./data:/app/data:rw" docker-compose.yml; then
    print_success "Volume do docker-compose configurado corretamente"
else
    print_error "Volume do docker-compose não está configurado corretamente"
fi

echo ""
print_success "✅ Teste de configuração concluído!"
echo ""
echo "Para testar a aplicação:"
echo "  docker compose down"
echo "  docker compose up --build"
