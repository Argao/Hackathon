#!/bin/bash

# ============================================================
# 🔧 K6 INSTALLATION SCRIPT
#   Instala o k6 para testes de performance
# ============================================================

set -e

# Cores para output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
CYAN='\033[0;36m'
NC='\033[0m' # No Color

# Função para log
log() {
    echo -e "${2:-$BLUE}$1${NC}"
}

# Função para detectar sistema operacional
detect_os() {
    if [[ "$OSTYPE" == "linux-gnu"* ]]; then
        if command -v apt-get &> /dev/null; then
            echo "ubuntu"
        elif command -v yum &> /dev/null; then
            echo "centos"
        elif command -v dnf &> /dev/null; then
            echo "fedora"
        else
            echo "linux"
        fi
    elif [[ "$OSTYPE" == "darwin"* ]]; then
        echo "macos"
    elif [[ "$OSTYPE" == "msys" ]] || [[ "$OSTYPE" == "cygwin" ]]; then
        echo "windows"
    else
        echo "unknown"
    fi
}

# Função para verificar se k6 já está instalado
check_k6_installed() {
    if command -v k6 &> /dev/null; then
        local version=$(k6 version | head -n 1)
        log "✅ k6 já está instalado: $version" "$GREEN"
        return 0
    else
        return 1
    fi
}

# Função para instalar no Ubuntu/Debian
install_ubuntu() {
    log "📦 Instalando k6 no Ubuntu/Debian..." "$CYAN"
    
    # Adicionar repositório oficial
    sudo gpg -k
    sudo gpg --no-default-keyring --keyring /usr/share/keyrings/k6-archive-keyring.gpg --keyserver hkp://keyserver.ubuntu.com:80 --recv-keys C5AD17C747E3415A3642D57D77C6C491D6AC1D69
    
    echo "deb [signed-by=/usr/share/keyrings/k6-archive-keyring.gpg] https://dl.k6.io/deb stable main" | sudo tee /etc/apt/sources.list.d/k6.list
    
    # Atualizar e instalar
    sudo apt-get update
    sudo apt-get install -y k6
    
    log "✅ k6 instalado com sucesso no Ubuntu/Debian" "$GREEN"
}

# Função para instalar no CentOS/RHEL
install_centos() {
    log "📦 Instalando k6 no CentOS/RHEL..." "$CYAN"
    
    # Adicionar repositório
    sudo yum install -y https://github.com/grafana/k6/releases/download/v0.47.0/k6-v0.47.0-linux-amd64.rpm
    
    log "✅ k6 instalado com sucesso no CentOS/RHEL" "$GREEN"
}

# Função para instalar no Fedora
install_fedora() {
    log "📦 Instalando k6 no Fedora..." "$CYAN"
    
    # Adicionar repositório
    sudo dnf install -y https://github.com/grafana/k6/releases/download/v0.47.0/k6-v0.47.0-linux-amd64.rpm
    
    log "✅ k6 instalado com sucesso no Fedora" "$GREEN"
}

# Função para instalar no macOS
install_macos() {
    log "📦 Instalando k6 no macOS..." "$CYAN"
    
    if command -v brew &> /dev/null; then
        brew install k6
        log "✅ k6 instalado com sucesso via Homebrew" "$GREEN"
    else
        log "❌ Homebrew não encontrado. Instale o Homebrew primeiro:" "$RED"
        log "   /bin/bash -c \"\$(curl -fsSL https://raw.githubusercontent.com/Homebrew/install/HEAD/install.sh)\"" "$YELLOW"
        exit 1
    fi
}

# Função para instalar no Windows
install_windows() {
    log "📦 Instalando k6 no Windows..." "$CYAN"
    
    if command -v choco &> /dev/null; then
        choco install k6
        log "✅ k6 instalado com sucesso via Chocolatey" "$GREEN"
    else
        log "❌ Chocolatey não encontrado. Instale o Chocolatey primeiro:" "$RED"
        log "   https://chocolatey.org/install" "$YELLOW"
        exit 1
    fi
}

# Função para instalação manual
install_manual() {
    log "📦 Instalando k6 manualmente..." "$CYAN"
    
    # Detectar arquitetura
    local arch=$(uname -m)
    local platform="linux"
    
    if [[ "$OSTYPE" == "darwin"* ]]; then
        platform="mac"
    fi
    
    # URL de download
    local version="v0.47.0"
    local url="https://github.com/grafana/k6/releases/download/${version}/k6-${version}-${platform}-${arch}.tar.gz"
    
    log "🔗 Baixando k6 de: $url" "$CYAN"
    
    # Criar diretório temporário
    local temp_dir=$(mktemp -d)
    cd "$temp_dir"
    
    # Baixar e extrair
    curl -L "$url" -o k6.tar.gz
    tar -xzf k6.tar.gz
    
    # Mover para PATH
    sudo mv k6-${version}-${platform}-${arch}/k6 /usr/local/bin/
    
    # Limpar
    cd - > /dev/null
    rm -rf "$temp_dir"
    
    log "✅ k6 instalado manualmente" "$GREEN"
}

# Função principal
main() {
    log "🚀 Iniciando instalação do k6..." "$GREEN"
    
    # Verificar se já está instalado
    if check_k6_installed; then
        log "ℹ️ k6 já está instalado. Pulando instalação." "$YELLOW"
        return 0
    fi
    
    # Detectar sistema operacional
    local os=$(detect_os)
    log "🖥️ Sistema operacional detectado: $os" "$CYAN"
    
    # Instalar baseado no sistema operacional
    case $os in
        "ubuntu")
            install_ubuntu
            ;;
        "centos")
            install_centos
            ;;
        "fedora")
            install_fedora
            ;;
        "macos")
            install_macos
            ;;
        "windows")
            install_windows
            ;;
        *)
            log "⚠️ Sistema operacional não suportado: $os" "$YELLOW"
            log "🔄 Tentando instalação manual..." "$CYAN"
            install_manual
            ;;
    esac
    
    # Verificar instalação
    if check_k6_installed; then
        log "🎉 k6 instalado com sucesso!" "$GREEN"
        log "📊 Versão: $(k6 version | head -n 1)" "$CYAN"
        log "🚀 Você pode agora executar os testes de performance:" "$GREEN"
        log "   ./scripts/performance-test.sh help" "$CYAN"
    else
        log "❌ Falha na instalação do k6" "$RED"
        log "🔧 Tente instalar manualmente:" "$YELLOW"
        log "   https://k6.io/docs/getting-started/installation/" "$CYAN"
        exit 1
    fi
}

# Executar função principal
main "$@"
