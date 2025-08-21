# 🐳 Docker Setup - Hackathon API

## Visão Geral

Este projeto inclui uma configuração Docker otimizada para a aplicação Hackathon.API, com suporte tanto para desenvolvimento quanto produção.

## Arquivos Docker

- `Dockerfile` - Imagem otimizada multi-stage para produção
- `docker-compose.yml` - Configuração para produção
- `docker-compose.dev.yml` - Configuração para desenvolvimento
- `.dockerignore` - Otimização do contexto de build

## 🚀 Quick Start

### Produção

```bash
# Build e execução
docker-compose up --build -d

# Verificar logs
docker-compose logs -f hackathon-api

# Parar aplicação
docker-compose down
```

### Desenvolvimento

```bash
# Build e execução em modo dev
docker-compose -f docker-compose.dev.yml up --build -d

# Verificar logs
docker-compose -f docker-compose.dev.yml logs -f hackathon-api-dev
```

## 📋 Endpoints

- **Aplicação**: `http://localhost:8080`
- **Health Check**: `http://localhost:8080/health`
- **Swagger** (dev): `http://localhost:5000/swagger`

## 🔧 Configurações

### Variáveis de Ambiente

```bash
ASPNETCORE_ENVIRONMENT=Production
ASPNETCORE_URLS=http://+:8080
ConnectionStrings__LocalDb=/app/data/hack.db
```

### Recursos do Container

- **CPU**: 1.0 core (limite), 0.5 core (reservado)
- **Memória**: 512MB (limite), 256MB (reservado)
- **Porta**: 8080 (não-privilegiada)

## 🏥 Health Checks

O container inclui health checks automáticos que verificam:
- Disponibilidade da aplicação na porta 8080
- Endpoint `/health` respondendo corretamente

## 📊 Volumes

- `hackathon-data`: Dados persistentes do banco SQLite (produção)
- `hackathon-data-dev`: Dados de desenvolvimento

## 🔐 Segurança

- Container executa como usuário não-root (`hackathon`)
- Porta não-privilegiada (8080)
- Imagem baseada em `mcr.microsoft.com/dotnet/aspnet:8.0`

## 🛠️ Comandos Úteis

```bash
# Ver status dos containers
docker-compose ps

# Executar comando no container
docker-compose exec hackathon-api bash

# Ver logs em tempo real
docker-compose logs -f

# Rebuild sem cache
docker-compose build --no-cache

# Limpar volumes (cuidado!)
docker-compose down -v
```

## 🔍 Troubleshooting

### Container não inicia
1. Verificar logs: `docker-compose logs hackathon-api`
2. Verificar health check: `docker inspect hackathon-api`
3. Testar endpoint manualmente: `curl http://localhost:8080/health`

### Problemas de permissão
- Verificar se o usuário `hackathon` tem permissões adequadas
- Verificar ownership dos volumes

### Performance
- Ajustar limites de CPU/memória no docker-compose.yml
- Verificar uso de recursos: `docker stats`

## 🔄 CI/CD

Para integração com pipelines:

```bash
# Build da imagem
docker build -t hackathon-api:latest .

# Tag para registry
docker tag hackathon-api:latest registry.example.com/hackathon-api:latest

# Push para registry
docker push registry.example.com/hackathon-api:latest
```
