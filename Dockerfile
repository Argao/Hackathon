# ===========================
# Dockerfile Otimizado para Hackathon.API
# ===========================

# ===========================
# Stage 1: Build Environment
# ===========================
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copiar arquivos de projeto primeiro para otimizar cache
COPY ["Hackathon.API/Hackathon.API.csproj", "Hackathon.API/"]
COPY ["Hackathon.Application/Hackathon.Application.csproj", "Hackathon.Application/"]
COPY ["Hackathon.Infrastructure/Hackathon.Infrastructure.csproj", "Hackathon.Infrastructure/"]
COPY ["Hackathon.Domain/Hackathon.Domain.csproj", "Hackathon.Domain/"]
COPY ["global.json", "./"]

# Restore das dependências de todos os projetos necessários
RUN dotnet restore "Hackathon.Domain/Hackathon.Domain.csproj" && \
    dotnet restore "Hackathon.Application/Hackathon.Application.csproj" && \
    dotnet restore "Hackathon.Infrastructure/Hackathon.Infrastructure.csproj" && \
    dotnet restore "Hackathon.API/Hackathon.API.csproj"

# Copiar todo o código fonte
COPY . .

# Build e Publish em um único stage
WORKDIR "/src/Hackathon.API"
RUN dotnet publish "Hackathon.API.csproj" -c Release -o /app/publish \
    --verbosity minimal

# ===========================
# Stage 2: Runtime
# ===========================
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final

# Definir usuário não-root para segurança
RUN groupadd -r hackathon && useradd -r -g hackathon hackathon

# Instalar dependências do sistema necessárias
RUN apt-get update && apt-get install -y \
    curl \
    && rm -rf /var/lib/apt/lists/* \
    && apt-get clean

WORKDIR /app

# Copiar aplicação publicada
COPY --from=build --chown=hackathon:hackathon /app/publish .

# Configurar variáveis de ambiente otimizadas
ENV ASPNETCORE_ENVIRONMENT=Production \
    ASPNETCORE_URLS=http://+:8080 \
    DOTNET_RUNNING_IN_CONTAINER=true \
    DOTNET_USE_POLLING_FILE_WATCHER=true \
    DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false

# Expor porta 8080 (não-privilegiada)
EXPOSE 8080

# Adicionar health check
HEALTHCHECK --interval=30s --timeout=10s --start-period=5s --retries=3 \
    CMD curl --fail http://localhost:8080/health || exit 1

# Script de inicialização

# Script de inicialização que aplica migrations e inicia a aplicação
COPY --chown=hackathon:hackathon scripts/start.sh /app/start.sh
RUN chmod +x /app/start.sh

# Mudar para usuário não-root
USER hackathon

# Ponto de entrada
ENTRYPOINT ["/app/start.sh"]
