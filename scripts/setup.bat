@echo off
REM ===========================
REM Script de Setup - Hackathon API (Windows)
REM ===========================

echo ================================
echo   Setup do Ambiente Hackathon API
echo ================================

REM Verificar se .NET 8 está instalado
echo ⚠ Verificando .NET 8...
dotnet --version >nul 2>&1
if %errorlevel% neq 0 (
    echo ✗ .NET não encontrado
    echo ⚠ Instale .NET 8: https://dotnet.microsoft.com/download/dotnet/8.0
    pause
    exit /b 1
)

for /f "tokens=*" %%i in ('dotnet --version') do set DOTNET_VERSION=%%i
echo ✓ .NET %DOTNET_VERSION% encontrado

REM Configurar pasta data
echo ⚠ Configurando pasta data...
if not exist "data" (
    mkdir data
    echo ✓ Pasta data criada
)

echo ✓ Permissões configuradas para Windows

REM Restaurar dependências
echo ⚠ Restaurando dependências...
dotnet restore
if %errorlevel% neq 0 (
    echo ✗ Erro ao restaurar dependências
    pause
    exit /b 1
)
echo ✓ Dependências restauradas

REM Verificar Docker (opcional)
echo ⚠ Verificando Docker...
docker info >nul 2>&1
if %errorlevel% equ 0 (
    echo ✓ Docker está rodando
) else (
    echo ⚠ Docker não encontrado ou não está rodando (opcional)
)

echo ================================
echo   Setup Concluído!
echo ================================
echo ✓ Agora você pode:
echo   • Rodar local: cd Hackathon.API ^&^& dotnet run
echo   • Rodar Docker: ./scripts/docker.sh dev
echo   • Acessar: http://localhost:5000 (local) ou http://localhost:8080 (Docker)

pause
