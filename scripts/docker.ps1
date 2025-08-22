# ===========================
# Script Docker - Hackathon API (PowerShell para Windows)
# ===========================

param(
    [Parameter(Position=0)]
    [string]$Command = "prod"
)

# Configurar para parar em caso de erro
$ErrorActionPreference = "Stop"

# Funções para output colorido
function Write-Header {
    param([string]$Message)
    Write-Host "================================" -ForegroundColor Blue
    Write-Host "  $Message" -ForegroundColor Blue
    Write-Host "================================" -ForegroundColor Blue
}

function Write-Success {
    param([string]$Message)
    Write-Host "✓ $Message" -ForegroundColor Green
}

function Write-Warning {
    param([string]$Message)
    Write-Host "⚠ $Message" -ForegroundColor Yellow
}

function Write-Error {
    param([string]$Message)
    Write-Host "✗ $Message" -ForegroundColor Red
}

# Função para produção
function Start-Production {
    Write-Header "Executando em Produção"
    
    Write-Warning "Parando containers existentes..."
    docker compose down --remove-orphans 2>$null
    
    Write-Warning "Iniciando aplicação..."
    docker compose up --build -d
    
    Write-Warning "Aguardando containers..."
    Start-Sleep -Seconds 15
    
    # Verificar status
    Write-Host "`nStatus dos containers:" -ForegroundColor Yellow
    docker compose ps
    
    # Testar health check
    Write-Host "`nTestando health check..." -ForegroundColor Yellow
    Start-Sleep -Seconds 5
    
    try {
        $response = Invoke-WebRequest -Uri "http://localhost:8080/health" -UseBasicParsing -TimeoutSec 10
        if ($response.StatusCode -eq 200) {
            Write-Success "Aplicação está rodando!"
            Write-Success "URL: http://localhost:8080"
            Write-Success "Health: http://localhost:8080/health"
        } else {
            Write-Error "Health check falhou"
        }
    }
    catch {
        Write-Error "Health check falhou"
        Write-Host "`nLogs da aplicação:" -ForegroundColor Yellow
        docker compose logs hackathon-api
    }
}

# Função para desenvolvimento
function Start-Development {
    Write-Header "Executando em Desenvolvimento"
    
    Write-Warning "Parando containers existentes..."
    docker compose --profile dev down --remove-orphans 2>$null
    
    Write-Warning "Iniciando aplicação (dev)..."
    docker compose --profile dev up --build -d
    
    Write-Warning "Aguardando containers..."
    Start-Sleep -Seconds 15
    
    # Verificar status
    Write-Host "`nStatus dos containers (dev):" -ForegroundColor Yellow
    docker compose --profile dev ps
    
    # Testar health check
    Write-Host "`nTestando health check..." -ForegroundColor Yellow
    Start-Sleep -Seconds 5
    
    try {
        $response = Invoke-WebRequest -Uri "http://localhost:5000/health" -UseBasicParsing -TimeoutSec 10
        if ($response.StatusCode -eq 200) {
            Write-Success "Aplicação está rodando!"
            Write-Success "URL: http://localhost:5000"
            Write-Success "Health: http://localhost:5000/health"
            Write-Success "Swagger: http://localhost:5000/swagger"
        } else {
            Write-Error "Health check falhou"
        }
    }
    catch {
        Write-Error "Health check falhou"
        Write-Host "`nLogs da aplicação:" -ForegroundColor Yellow
        docker compose --profile dev logs hackathon-api-dev
    }
}

# Função para parar
function Stop-Services {
    Write-Header "Parando Serviços"
    
    Write-Warning "Parando produção..."
    docker compose down 2>$null
    
    Write-Warning "Parando desenvolvimento..."
    docker compose --profile dev down 2>$null
    
    Write-Success "Todos os serviços foram parados"
}

# Função para logs
function Show-Logs {
    param([string]$Environment = "prod")
    
    Write-Header "Logs - $Environment"
    
    switch ($Environment.ToLower()) {
        "prod" { docker compose logs -f hackathon-api }
        "production" { docker compose logs -f hackathon-api }
        "dev" { docker compose --profile dev logs -f hackathon-api-dev }
        "development" { docker compose --profile dev logs -f hackathon-api-dev }
        default {
            Write-Error "Ambiente inválido: $Environment"
            exit 1
        }
    }
}

# Função para status
function Show-Status {
    Write-Header "Status dos Containers"
    
    Write-Host "Produção:" -ForegroundColor Yellow
    try {
        docker compose ps
    }
    catch {
        Write-Host "Nenhum container de produção rodando"
    }
    
    Write-Host "`nDesenvolvimento:" -ForegroundColor Yellow
    try {
        docker compose --profile dev ps
    }
    catch {
        Write-Host "Nenhum container de desenvolvimento rodando"
    }
    
    Write-Host "`nTodos os containers Docker:" -ForegroundColor Yellow
    docker ps --filter label=com.docker.compose.project=hackathon --format "table {{.Names}}\t{{.Status}}\t{{.Ports}}"
}

# Função para limpeza
function Clear-All {
    Write-Header "Limpeza Completa"
    
    Write-Warning "Parando todos os serviços..."
    Stop-Services
    
    Write-Warning "Removendo volumes..."
    docker volume rm hackathon-data 2>$null
    docker volume rm hackathon-data-dev 2>$null
    
    Write-Warning "Removendo imagens..."
    docker rmi hackathon-api:latest 2>$null
    docker rmi hackathon-api:dev 2>$null
    
    Write-Warning "Removendo redes..."
    docker network rm hackathon-network 2>$null
    
    Write-Success "Limpeza concluída"
}

# Função para mostrar ajuda
function Show-Help {
    Write-Host "Uso: .\docker.ps1 [prod|dev|stop|logs|status|clean|help] [env]"
    Write-Host ""
    Write-Host "Comandos:"
    Write-Host "  prod        - Executa em produção (padrão)"
    Write-Host "  dev         - Executa em desenvolvimento"
    Write-Host "  stop        - Para todos os serviços"
    Write-Host "  logs [env]  - Mostra logs (prod|dev)"
    Write-Host "  status      - Mostra status dos containers"
    Write-Host "  clean       - Limpeza completa (remove tudo)"
    Write-Host "  help        - Mostra esta ajuda"
    Write-Host ""
    Write-Host "Exemplos:"
    Write-Host "  .\docker.ps1 prod      # Executa em produção"
    Write-Host "  .\docker.ps1 dev       # Executa em desenvolvimento"
    Write-Host "  .\docker.ps1 logs dev  # Logs do ambiente de desenvolvimento"
}

# Função principal
function Main {
    # Verificar se Docker está rodando
    try {
        docker info | Out-Null
    }
    catch {
        Write-Error "Docker não está rodando ou não está acessível"
        exit 1
    }
    
    # Verificar se estamos no diretório correto
    if (-not (Test-Path "docker-compose.yml")) {
        Write-Error "docker-compose.yml não encontrado. Execute este script na raiz do projeto."
        exit 1
    }
    
    # Processar comando
    switch ($Command.ToLower()) {
        "prod" { Start-Production }
        "production" { Start-Production }
        "dev" { Start-Development }
        "development" { Start-Development }
        "stop" { Stop-Services }
        "logs" { 
            $env = $args[0] ?? "prod"
            Show-Logs $env 
        }
        "status" { Show-Status }
        "clean" { Clear-All }
        "help" { Show-Help }
        "-h" { Show-Help }
        "--help" { Show-Help }
        default {
            Write-Error "Comando inválido: $Command"
            Write-Host "Use '.\docker.ps1 help' para ver os comandos disponíveis"
            exit 1
        }
    }
}

# Executar função principal
Main
