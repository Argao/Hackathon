# ===========================
# Script de Setup - Hackathon API (Windows)
# ===========================

param(
    [switch]$Force
)

# Cores para output
$Green = "Green"
$Yellow = "Yellow"
$Red = "Red"

function Write-Success {
    param([string]$Message)
    Write-Host "✓ $Message" -ForegroundColor $Green
}

function Write-Warning {
    param([string]$Message)
    Write-Host "⚠ $Message" -ForegroundColor $Yellow
}

function Write-Error {
    param([string]$Message)
    Write-Host "✗ $Message" -ForegroundColor $Red
}

function Write-Header {
    param([string]$Message)
    Write-Host "================================`n  $Message`n================================" -ForegroundColor $Yellow
}

# Verificar se .NET 8 está instalado
function Test-DotNet {
    Write-Warning "Verificando .NET 8..."
    
    try {
        $dotnetVersion = dotnet --version
        if ($dotnetVersion -like "8.*") {
            Write-Success ".NET $dotnetVersion encontrado"
        } else {
            Write-Error ".NET 8 não encontrado. Versão atual: $dotnetVersion"
            Write-Warning "Instale .NET 8: https://dotnet.microsoft.com/download/dotnet/8.0"
            exit 1
        }
    } catch {
        Write-Error ".NET não encontrado"
        Write-Warning "Instale .NET 8: https://dotnet.microsoft.com/download/dotnet/8.0"
        exit 1
    }
}

# Configurar pasta data
function Setup-DataFolder {
    Write-Warning "Configurando pasta data..."
    
    # Criar pasta se não existir
    if (-not (Test-Path "data")) {
        New-Item -ItemType Directory -Path "data" -Force | Out-Null
        Write-Success "Pasta data criada"
    }
    
    # Ajustar permissões (Windows não precisa de chmod)
    Write-Success "Permissões configuradas para Windows"
}

# Restaurar dependências
function Restore-Dependencies {
    Write-Warning "Restaurando dependências..."
    
    dotnet restore
    Write-Success "Dependências restauradas"
}

# Verificar Docker (opcional)
function Test-Docker {
    Write-Warning "Verificando Docker..."
    
    try {
        docker info | Out-Null
        Write-Success "Docker está rodando"
    } catch {
        Write-Warning "Docker não encontrado ou não está rodando (opcional para desenvolvimento local)"
    }
}

# Função principal
function Main {
    Write-Header "Setup do Ambiente Hackathon API (Windows)"
    
    Test-DotNet
    Setup-DataFolder
    Restore-Dependencies
    Test-Docker
    
    Write-Header "Setup Concluído!"
    Write-Success "Agora você pode:"
    Write-Host "  • Rodar local: cd Hackathon.API && dotnet run"
    Write-Host "  • Rodar Docker: ./scripts/docker.sh dev"
    Write-Host "  • Acessar: http://localhost:5000 (local) ou http://localhost:8080 (Docker)"
}

# Executar função principal
Main
