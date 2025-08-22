#!/bin/bash

# ===========================
# Script de Setup - Hackathon API
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

print_header() {
    echo -e "${YELLOW}================================${NC}"
    echo -e "${YELLOW}  $1${NC}"
    echo -e "${YELLOW}================================${NC}"
}

# Verificar se .NET 8 está instalado
check_dotnet() {
    print_warning "Verificando .NET 8..."
    
    if command -v dotnet &> /dev/null; then
        DOTNET_VERSION=$(dotnet --version)
        if [[ $DOTNET_VERSION == 8.* ]]; then
            print_success ".NET $DOTNET_VERSION encontrado"
        else
            print_error ".NET 8 não encontrado. Versão atual: $DOTNET_VERSION"
            print_warning "Instale .NET 8: https://dotnet.microsoft.com/download/dotnet/8.0"
            exit 1
        fi
    else
        print_error ".NET não encontrado"
        print_warning "Instale .NET 8: https://dotnet.microsoft.com/download/dotnet/8.0"
        exit 1
    fi
}

# Configurar pasta data
setup_data_folder() {
    print_warning "Configurando pasta data..."
    
    # Criar pasta se não existir
    if [ ! -d "data" ]; then
        mkdir -p data
        print_success "Pasta data criada"
    fi
    
    # Ajustar permissões
    if [[ "$OSTYPE" == "linux-gnu"* ]]; then
        # Linux
        chmod 777 data/ 2>/dev/null || sudo chmod 777 data/
        chown -R $(whoami):$(whoami) data/ 2>/dev/null || sudo chown -R $(whoami):$(whoami) data/
        print_success "Permissões ajustadas para Linux"
    elif [[ "$OSTYPE" == "darwin"* ]]; then
        # macOS
        chmod 755 data/
        print_success "Permissões ajustadas para macOS"
    else
        # Windows ou outros
        chmod 755 data/ 2>/dev/null || true
        print_success "Permissões ajustadas"
    fi
}

# Restaurar dependências
restore_dependencies() {
    print_warning "Restaurando dependências..."
    
    dotnet restore
    print_success "Dependências restauradas"
}

# Verificar Docker (opcional)
check_docker() {
    print_warning "Verificando Docker..."
    
    if command -v docker &> /dev/null; then
        if docker info &> /dev/null; then
            print_success "Docker está rodando"
        else
            print_warning "Docker instalado mas não está rodando"
        fi
    else
        print_warning "Docker não encontrado (opcional para desenvolvimento local)"
    fi
}

# Função principal
main() {
    print_header "Setup do Ambiente Hackathon API"
    
    check_dotnet
    setup_data_folder
    restore_dependencies
    check_docker
    
    print_header "Setup Concluído!"
    print_success "Agora você pode:"
    echo "  • Rodar local: cd Hackathon.API && dotnet run"
    echo "  • Rodar Docker: ./scripts/docker.sh dev"
    echo "  • Acessar: http://localhost:5000 (local) ou http://localhost:8080 (Docker)"
}

# Executar função principal
main "$@"
