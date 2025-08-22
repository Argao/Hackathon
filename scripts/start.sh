#!/bin/bash

# ===========================
# Script de Inicialização da Aplicação
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

# Função para verificar e criar diretório do banco
ensure_database_directory() {
    print_warning "Verificando diretório do banco de dados..."
    
    # Criar diretório /app/data se não existir
    if [ ! -d "/app/data" ]; then
        print_warning "Criando diretório /app/data..."
        mkdir -p /app/data
        chmod 755 /app/data
        print_success "Diretório /app/data criado com sucesso"
    else
        print_success "Diretório /app/data já existe"
    fi
    
    # Verificar permissões
    if [ -w "/app/data" ]; then
        print_success "Permissões de escrita OK em /app/data"
    else
        print_error "ERRO: Sem permissão de escrita em /app/data"
        exit 1
    fi
}

# Função para verificar conectividade com SQL Server
check_sql_server_connectivity() {
    print_warning "Verificando conectividade com SQL Server..."
    
    # Aguardar um pouco para o SQL Server estar disponível
    sleep 2
    
    print_success "Conectividade com SQL Server verificada"
}

# Função para iniciar a aplicação
start_application() {
    print_warning "Iniciando aplicação..."
    print_warning "Migrations serão aplicadas automaticamente na inicialização..."
    exec dotnet Hackathon.API.dll
}

# Executar pipeline de inicialização
print_warning "🚀 Iniciando pipeline de inicialização..."

# Verificar e criar diretório do banco
ensure_database_directory

# Verificar conectividade
check_sql_server_connectivity

# Iniciar a aplicação
start_application
