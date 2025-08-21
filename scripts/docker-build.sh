#!/bin/bash

# ===========================
# Script de Build Docker - Hackathon API
# ===========================

set -e

# Cores para output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m' # No Color

# Configurações
IMAGE_NAME="hackathon-api"
CONTAINER_NAME="hackathon-api"

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

# Função para limpeza
cleanup() {
    print_header "Limpeza"
    
    # Parar e remover container se existir
    if docker ps -a --format 'table {{.Names}}' | grep -q "^${CONTAINER_NAME}$"; then
        print_warning "Removendo container existente: ${CONTAINER_NAME}"
        docker stop ${CONTAINER_NAME} 2>/dev/null || true
        docker rm ${CONTAINER_NAME} 2>/dev/null || true
    fi
    
    # Remover imagem antiga se existir
    if docker images --format 'table {{.Repository}}:{{.Tag}}' | grep -q "^${IMAGE_NAME}:latest$"; then
        print_warning "Removendo imagem antiga: ${IMAGE_NAME}:latest"
        docker rmi ${IMAGE_NAME}:latest 2>/dev/null || true
    fi
    
    print_success "Limpeza concluída"
}

# Função para build
build_image() {
    print_header "Build da Imagem Docker"
    
    echo "Building ${IMAGE_NAME}:latest..."
    
    # Build com cache otimizado
    docker build \
        --build-arg BUILDKIT_INLINE_CACHE=1 \
        --tag ${IMAGE_NAME}:latest \
        --file Dockerfile \
        . || {
            print_error "Falha no build da imagem"
            exit 1
        }
    
    print_success "Imagem construída com sucesso: ${IMAGE_NAME}:latest"
}

# Função para verificar imagem
verify_image() {
    print_header "Verificação da Imagem"
    
    # Verificar se a imagem foi criada
    if ! docker images --format 'table {{.Repository}}:{{.Tag}}' | grep -q "^${IMAGE_NAME}:latest$"; then
        print_error "Imagem não encontrada após o build"
        exit 1
    fi
    
    # Mostrar tamanho da imagem
    IMAGE_SIZE=$(docker images --format "table {{.Size}}" ${IMAGE_NAME}:latest | tail -n 1)
    print_success "Imagem verificada - Tamanho: ${IMAGE_SIZE}"
    
    # Mostrar layers da imagem
    echo -e "\n${YELLOW}Layers da imagem:${NC}"
    docker history ${IMAGE_NAME}:latest --format "table {{.CreatedBy}}\t{{.Size}}" | head -10
}

# Função para executar
run_container() {
    print_header "Executando Container"
    
    docker run \
        --name ${CONTAINER_NAME} \
        --detach \
        --publish 8080:8080 \
        --env ASPNETCORE_ENVIRONMENT=Production \
        --env ASPNETCORE_URLS=http://+:8080 \
        --restart unless-stopped \
        ${IMAGE_NAME}:latest || {
            print_error "Falha ao executar o container"
            exit 1
        }
    
    print_success "Container iniciado: ${CONTAINER_NAME}"
    
    # Aguardar health check
    print_warning "Aguardando health check..."
    sleep 10
    
    # Verificar se o container está rodando
    if docker ps --format 'table {{.Names}}' | grep -q "^${CONTAINER_NAME}$"; then
        print_success "Container está executando corretamente"
        
        # Testar endpoint de health
        echo -e "\n${YELLOW}Testando health check...${NC}"
        sleep 5
        
        if curl -f http://localhost:8080/health >/dev/null 2>&1; then
            print_success "Health check passou!"
        else
            print_warning "Health check falhou - verificar logs"
        fi
        
        # Mostrar informações do container
        echo -e "\n${YELLOW}Informações do container:${NC}"
        docker ps --filter name=${CONTAINER_NAME} --format "table {{.Names}}\t{{.Status}}\t{{.Ports}}"
        
        echo -e "\n${GREEN}✓ Aplicação disponível em: http://localhost:8080${NC}"
        echo -e "${GREEN}✓ Health check em: http://localhost:8080/health${NC}"
        
    else
        print_error "Container não está executando"
        print_warning "Verificando logs..."
        docker logs ${CONTAINER_NAME}
        exit 1
    fi
}

# Função principal
main() {
    print_header "Build Docker - Hackathon API"
    
    # Verificar se Docker está rodando
    if ! docker info >/dev/null 2>&1; then
        print_error "Docker não está rodando ou não está acessível"
        exit 1
    fi
    
    # Verificar se estamos no diretório correto
    if [ ! -f "Dockerfile" ]; then
        print_error "Dockerfile não encontrado. Execute este script na raiz do projeto."
        exit 1
    fi
    
    # Parse argumentos
    case "${1:-build}" in
        "clean")
            cleanup
            ;;
        "build")
            cleanup
            build_image
            verify_image
            ;;
        "run")
            cleanup
            build_image
            verify_image
            run_container
            ;;
        "help"|"-h"|"--help")
            echo "Uso: $0 [clean|build|run|help]"
            echo ""
            echo "Comandos:"
            echo "  clean   - Remove imagens e containers antigos"
            echo "  build   - Build da imagem Docker (padrão)"
            echo "  run     - Build e executa o container"
            echo "  help    - Mostra esta ajuda"
            exit 0
            ;;
        *)
            print_error "Comando inválido: $1"
            echo "Use '$0 help' para ver os comandos disponíveis"
            exit 1
            ;;
    esac
    
    print_success "Processo concluído com sucesso!"
}

# Executar função principal
main "$@"
