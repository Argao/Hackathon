@echo off
setlocal enabledelayedexpansion

REM ===========================
REM Script Docker - Hackathon API (Batch para Windows)
REM ===========================

REM Configurar cores (se disponível)
set "BLUE=[94m"
set "GREEN=[92m"
set "YELLOW=[93m"
set "RED=[91m"
set "NC=[0m"

REM Função para header
:print_header
echo %BLUE%================================%NC%
echo %BLUE%  %~1%NC%
echo %BLUE%================================%NC%
goto :eof

REM Função para sucesso
:print_success
echo %GREEN%✓ %~1%NC%
goto :eof

REM Função para warning
:print_warning
echo %YELLOW%⚠ %~1%NC%
goto :eof

REM Função para erro
:print_error
echo %RED%✗ %~1%NC%
goto :eof

REM Função para produção
:run_production
call :print_header "Executando em Produção"
call :print_warning "Parando containers existentes..."
docker compose down --remove-orphans >nul 2>&1

call :print_warning "Iniciando aplicação..."
docker compose up --build -d

call :print_warning "Aguardando containers..."
timeout /t 15 /nobreak >nul

echo.
echo %YELLOW%Status dos containers:%NC%
docker compose ps

echo.
echo %YELLOW%Testando health check...%NC%
timeout /t 5 /nobreak >nul

REM Testar health check
powershell -Command "try { $response = Invoke-WebRequest -Uri 'http://localhost:8080/health' -UseBasicParsing -TimeoutSec 10; if ($response.StatusCode -eq 200) { Write-Host '✓ Aplicação está rodando!' -ForegroundColor Green; Write-Host '✓ URL: http://localhost:8080' -ForegroundColor Green; Write-Host '✓ Health: http://localhost:8080/health' -ForegroundColor Green } else { Write-Host '✗ Health check falhou' -ForegroundColor Red } } catch { Write-Host '✗ Health check falhou' -ForegroundColor Red; Write-Host 'Logs da aplicação:' -ForegroundColor Yellow; docker compose logs hackathon-api }"
goto :eof

REM Função para desenvolvimento
:run_development
call :print_header "Executando em Desenvolvimento"
call :print_warning "Parando containers existentes..."
docker compose --profile dev down --remove-orphans >nul 2>&1

call :print_warning "Iniciando aplicação (dev)..."
docker compose --profile dev up --build -d

call :print_warning "Aguardando containers..."
timeout /t 15 /nobreak >nul

echo.
echo %YELLOW%Status dos containers (dev):%NC%
docker compose --profile dev ps

echo.
echo %YELLOW%Testando health check...%NC%
timeout /t 5 /nobreak >nul

REM Testar health check
powershell -Command "try { $response = Invoke-WebRequest -Uri 'http://localhost:5000/health' -UseBasicParsing -TimeoutSec 10; if ($response.StatusCode -eq 200) { Write-Host '✓ Aplicação está rodando!' -ForegroundColor Green; Write-Host '✓ URL: http://localhost:5000' -ForegroundColor Green; Write-Host '✓ Health: http://localhost:5000/health' -ForegroundColor Green; Write-Host '✓ Swagger: http://localhost:5000/swagger' -ForegroundColor Green } else { Write-Host '✗ Health check falhou' -ForegroundColor Red } } catch { Write-Host '✗ Health check falhou' -ForegroundColor Red; Write-Host 'Logs da aplicação:' -ForegroundColor Yellow; docker compose --profile dev logs hackathon-api-dev }"
goto :eof

REM Função para parar
:stop_services
call :print_header "Parando Serviços"
call :print_warning "Parando produção..."
docker compose down >nul 2>&1

call :print_warning "Parando desenvolvimento..."
docker compose --profile dev down >nul 2>&1

call :print_success "Todos os serviços foram parados"
goto :eof

REM Função para logs
:show_logs
set "ENV=%~1"
if "%ENV%"=="" set "ENV=prod"

call :print_header "Logs - %ENV%"

if /i "%ENV%"=="prod" (
    docker compose logs -f hackathon-api
) else if /i "%ENV%"=="production" (
    docker compose logs -f hackathon-api
) else if /i "%ENV%"=="dev" (
    docker compose --profile dev logs -f hackathon-api-dev
) else if /i "%ENV%"=="development" (
    docker compose --profile dev logs -f hackathon-api-dev
) else (
    call :print_error "Ambiente inválido: %ENV%"
    exit /b 1
)
goto :eof

REM Função para status
:show_status
call :print_header "Status dos Containers"

echo %YELLOW%Produção:%NC%
docker compose ps 2>nul || echo Nenhum container de produção rodando

echo.
echo %YELLOW%Desenvolvimento:%NC%
docker compose --profile dev ps 2>nul || echo Nenhum container de desenvolvimento rodando

echo.
echo %YELLOW%Todos os containers Docker:%NC%
docker ps --filter label=com.docker.compose.project=hackathon --format "table {{.Names}}\t{{.Status}}\t{{.Ports}}"
goto :eof

REM Função para limpeza
:cleanup
call :print_header "Limpeza Completa"
call :print_warning "Parando todos os serviços..."
call :stop_services

call :print_warning "Removendo volumes..."
docker volume rm hackathon-data >nul 2>&1
docker volume rm hackathon-data-dev >nul 2>&1

call :print_warning "Removendo imagens..."
docker rmi hackathon-api:latest >nul 2>&1
docker rmi hackathon-api:dev >nul 2>&1

call :print_warning "Removendo redes..."
docker network rm hackathon-network >nul 2>&1

call :print_success "Limpeza concluída"
goto :eof

REM Função para mostrar ajuda
:show_help
echo Uso: docker.bat [prod^|dev^|stop^|logs^|status^|clean^|help] [env]
echo.
echo Comandos:
echo   prod        - Executa em produção (padrão^)
echo   dev         - Executa em desenvolvimento
echo   stop        - Para todos os serviços
echo   logs [env]  - Mostra logs (prod^|dev^)
echo   status      - Mostra status dos containers
echo   clean       - Limpeza completa (remove tudo^)
echo   help        - Mostra esta ajuda
echo.
echo Exemplos:
echo   docker.bat prod      # Executa em produção
echo   docker.bat dev       # Executa em desenvolvimento
echo   docker.bat logs dev  # Logs do ambiente de desenvolvimento
goto :eof

REM Função principal
:main
REM Verificar se Docker está rodando
docker info >nul 2>&1
if errorlevel 1 (
    call :print_error "Docker não está rodando ou não está acessível"
    exit /b 1
)

REM Verificar se estamos no diretório correto
if not exist "docker-compose.yml" (
    call :print_error "docker-compose.yml não encontrado. Execute este script na raiz do projeto."
    exit /b 1
)

REM Processar comando
set "COMMAND=%~1"
if "%COMMAND%"=="" set "COMMAND=prod"

if /i "%COMMAND%"=="prod" (
    call :run_production
) else if /i "%COMMAND%"=="production" (
    call :run_production
) else if /i "%COMMAND%"=="dev" (
    call :run_development
) else if /i "%COMMAND%"=="development" (
    call :run_development
) else if /i "%COMMAND%"=="stop" (
    call :stop_services
) else if /i "%COMMAND%"=="logs" (
    call :show_logs "%~2"
) else if /i "%COMMAND%"=="status" (
    call :show_status
) else if /i "%COMMAND%"=="clean" (
    call :cleanup
) else if /i "%COMMAND%"=="help" (
    call :show_help
) else if /i "%COMMAND%"=="-h" (
    call :show_help
) else if /i "%COMMAND%"=="--help" (
    call :show_help
) else (
    call :print_error "Comando inválido: %COMMAND%"
    echo Use 'docker.bat help' para ver os comandos disponíveis
    exit /b 1
)

goto :eof

REM Executar função principal
call :main %*
