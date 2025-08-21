#!/bin/bash

# ===========================
# Script Docker Compose - Hackathon API
# ===========================

set -e

# Cores para output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m' # No Color

print_header() {
    echo -e "${BLUE}================================${NC}"
    echo -e "${BLUE}  $1${NC}"
    echo -e "${BLUE}================================${NC}"
}

print_success() {
    echo -e "${GREEN}✓ $1${NC}"
}

print_warning() {
    echo -e "${YELLOW}⚠ $1${NC}"
}

print_error() {
    echo -e "${RED}✗ $1${NC}"
}

# Função para produção
run_production() {
    print_header "Executando em Produção"
    
    print_warning "Parando containers existentes..."
    docker-compose down --remove-orphans 2>/dev/null || true
    
    print_warning "Iniciando aplicação..."
    docker-compose up --build -d
    
    print_warning "Aguardando containers..."
    sleep 15
    
    # Verificar status
    echo -e "\n${YELLOW}Status dos containers:${NC}"
    docker-compose ps
    
    # Testar health check
    echo -e "\n${YELLOW}Testando health check...${NC}"
    sleep 5
    
    if curl -f http://localhost:8080/health >/dev/null 2>&1; then
        print_success "Aplicação está rodando!"
        print_success "URL: http://localhost:8080"
        print_success "Health: http://localhost:8080/health"
    else
        print_error "Health check falhou"
        echo -e "\n${YELLOW}Logs da aplicação:${NC}"
        docker-compose logs hackathon-api
    fi
}

# Função para desenvolvimento
run_development() {
    print_header "Executando em Desenvolvimento"
    
    print_warning "Parando containers existentes..."
    docker-compose -f docker-compose.dev.yml down --remove-orphans 2>/dev/null || true
    
    print_warning "Iniciando aplicação (dev)..."
    docker-compose -f docker-compose.dev.yml up --build -d
    
    print_warning "Aguardando containers..."
    sleep 15
    
    # Verificar status
    echo -e "\n${YELLOW}Status dos containers (dev):${NC}"
    docker-compose -f docker-compose.dev.yml ps
    
    # Testar health check
    echo -e "\n${YELLOW}Testando health check...${NC}"
    sleep 5
    
    if curl -f http://localhost:5000/health >/dev/null 2>&1; then
        print_success "Aplicação está rodando!"
        print_success "URL: http://localhost:5000"
        print_success "Health: http://localhost:5000/health"
        print_success "Swagger: http://localhost:5000/swagger"
    else
        print_error "Health check falhou"
        echo -e "\n${YELLOW}Logs da aplicação:${NC}"
        docker-compose -f docker-compose.dev.yml logs hackathon-api-dev
    fi
}

# Função para parar
stop_services() {
    print_header "Parando Serviços"
    
    print_warning "Parando produção..."
    docker-compose down 2>/dev/null || true
    
    print_warning "Parando desenvolvimento..."
    docker-compose -f docker-compose.dev.yml down 2>/dev/null || true
    
    print_success "Todos os serviços foram parados"
}

# Função para logs
show_logs() {
    local ENV=${1:-prod}
    
    print_header "Logs - ${ENV}"
    
    case "$ENV" in
        "prod"|"production")
            docker-compose logs -f hackathon-api
            ;;
        "dev"|"development")
            docker-compose -f docker-compose.dev.yml logs -f hackathon-api-dev
            ;;
        *)
            print_error "Ambiente inválido: $ENV"
            exit 1
            ;;
    esac
}

# Função para status
show_status() {
    print_header "Status dos Containers"
    
    echo -e "${YELLOW}Produção:${NC}"
    docker-compose ps 2>/dev/null || echo "Nenhum container de produção rodando"
    
    echo -e "\n${YELLOW}Desenvolvimento:${NC}"
    docker-compose -f docker-compose.dev.yml ps 2>/dev/null || echo "Nenhum container de desenvolvimento rodando"
    
    echo -e "\n${YELLOW}Todos os containers Docker:${NC}"
    docker ps --filter label=com.docker.compose.project=hackathon --format "table {{.Names}}\t{{.Status}}\t{{.Ports}}"
}

# Função para limpeza
cleanup() {
    print_header "Limpeza Completa"
    
    print_warning "Parando todos os serviços..."
    stop_services
    
    print_warning "Removendo volumes..."
    docker volume rm hackathon-data 2>/dev/null || true
    docker volume rm hackathon-data-dev 2>/dev/null || true
    
    print_warning "Removendo imagens..."
    docker rmi hackathon-api:latest 2>/dev/null || true
    docker rmi hackathon-api:dev 2>/dev/null || true
    
    print_warning "Removendo redes..."
    docker network rm hackathon-network 2>/dev/null || true
    docker network rm hackathon-dev-network 2>/dev/null || true
    
    print_success "Limpeza concluída"
}

# Função principal
main() {
    # Verificar se Docker está rodando
    if ! docker info >/dev/null 2>&1; then
        print_error "Docker não está rodando ou não está acessível"
        exit 1
    fi
    
    # Verificar se estamos no diretório correto
    if [ ! -f "docker-compose.yml" ]; then
        print_error "docker-compose.yml não encontrado. Execute este script na raiz do projeto."
        exit 1
    fi
    
    case "${1:-prod}" in
        "prod"|"production")
            run_production
            ;;
        "dev"|"development")
            run_development
            ;;
        "stop")
            stop_services
            ;;
        "logs")
            show_logs "${2:-prod}"
            ;;
        "status")
            show_status
            ;;
        "clean")
            cleanup
            ;;
        "help"|"-h"|"--help")
            echo "Uso: $0 [prod|dev|stop|logs|status|clean|help] [env]"
            echo ""
            echo "Comandos:"
            echo "  prod        - Executa em produção (padrão)"
            echo "  dev         - Executa em desenvolvimento"
            echo "  stop        - Para todos os serviços"
            echo "  logs [env]  - Mostra logs (prod|dev)"
            echo "  status      - Mostra status dos containers"
            echo "  clean       - Limpeza completa (remove tudo)"
            echo "  help        - Mostra esta ajuda"
            echo ""
            echo "Exemplos:"
            echo "  $0 prod      # Executa em produção"
            echo "  $0 dev       # Executa em desenvolvimento"
            echo "  $0 logs dev  # Logs do ambiente de desenvolvimento"
            exit 0
            ;;
        *)
            print_error "Comando inválido: $1"
            echo "Use '$0 help' para ver os comandos disponíveis"
            exit 1
            ;;
    esac
}

# Executar função principal
main "$@"
