# 🚀 Hackathon Credit Simulation API

> **Uma API de simulação de crédito robusta, escalável e production-ready construída com .NET 8**

[![.NET](https://img.shields.io/badge/.NET-8.0-blue.svg)](https://dotnet.microsoft.com/download/dotnet/8.0)
[![License](https://img.shields.io/badge/license-MIT-green.svg)](LICENSE)
[![Docker](https://img.shields.io/badge/docker-ready-blue.svg)](Dockerfile)

## 🎯 **Por que escolher esta API?**

### ✨ **Funcionalidades Principais**

- **🧮 Cálculos Financeiros Precisos**: Implementa **SAC** e **PRICE** com arredondamento financeiro adequado
- **📊 Telemetria Automática**: Coleta métricas de performance de todos os endpoints automaticamente
- **⚡ Performance Otimizada**: Cache inteligente de produtos + warm-up automático = **latência ZERO**
- **🏗️ Arquitetura Limpa**: Clean Architecture com separação clara de responsabilidades
- **🐳 Docker Production-Ready**: Containerização otimizada com health checks e usuário não-root
- **📈 Analytics**: Relatórios de volume simulado por produto e data
- **🛡️ Tratamento de Erros**: Global exception handling com respostas estruturadas
- **✅ Testes Abrangentes**: Cobertura completa com mais de 50 arquivos de teste

### 🏆 **Qualidades Técnicas**

- **Domain-Driven Design (DDD)**: Value Objects, Entities e Services bem definidos
- **CQRS Pattern**: Commands e Queries separados para operações de escrita e leitura
- **Repository Pattern**: Abstração de acesso a dados para flexibilidade
- **Dependency Injection**: Inversão de controle total com container nativo do .NET
- **Mapster**: Mapeamento de objetos otimizado e performático
- **FluentValidation**: Validações robustas e reutilizáveis
- **Health Checks**: Monitoramento automático da saúde da aplicação

## 🛠️ **Tecnologias Utilizadas**

- **.NET 8** - Framework principal
- **ASP.NET Core** - Web API
- **Entity Framework Core** - ORM para SQL Server e SQLite
- **SQLite** - Banco local para simulações e métricas
- **SQL Server** - Banco externo para produtos
- **Azure Event Hub** - Mensageria para analytics
- **Mapster** - Object mapping
- **FluentValidation** - Validação de dados
- **Swagger/OpenAPI** - Documentação automática

## 📋 **Pré-requisitos**

### Para Execução Local
- **[.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)** (versão 8.0.0 ou superior)

### Para Execução com Docker
- **[Docker](https://docs.docker.com/get-docker/)** e **[Docker Compose](https://docs.docker.com/compose/install/)**

## 🚀 **Quick Start**

### 🛠️ **Setup Inicial (Obrigatório)**

Execute o script de setup para configurar o ambiente:

```bash
# Linux/macOS
chmod +x scripts/setup.sh
./scripts/setup.sh

# Windows (PowerShell)
scripts\setup.ps1

# Windows (Batch)
scripts\setup.bat
```

**O script de setup vai:**
- ✅ Verificar se .NET 8 está instalado
- ✅ Criar e configurar a pasta `data/` com permissões adequadas
- ✅ Restaurar todas as dependências do projeto
- ✅ Verificar se Docker está disponível

### 🏃‍♂️ **Execução Local (Desenvolvimento)**

```bash
# Após executar o setup
cd Hackathon.API
dotnet run

# Ou direto da raiz
dotnet run --project Hackathon.API
```

**Acesse em:** http://localhost:5000
**Swagger UI:** http://localhost:5000/swagger

### 🐳 **Execução com Docker (Produção)**

```bash
# Produção (recomendado)
./scripts/docker-compose.sh prod

# Desenvolvimento (com hot reload)
./scripts/docker-compose.sh dev

# Ver logs
./scripts/docker-compose.sh logs

# Parar serviços
./scripts/docker-compose.sh stop
```

**Acesse em:** http://localhost:8080
**Swagger UI:** http://localhost:8080/swagger

## 📖 **Como Usar a API**

### 🧮 **Realizar Simulação de Crédito**

```http
POST /simulacao
Content-Type: application/json

{
  "valorDesejado": 10000.00,
  "prazo": 12
}
```

**Resposta:** Cálculos completos para **SAC** e **PRICE** com todas as parcelas detalhadas.

### 📋 **Listar Simulações (Paginado)**

```http
GET /simulacao?pagina=1&tamanhoPagina=10
```

### 📊 **Volume Simulado por Dia**

```http
GET /simulacao/volume-por-dia?dataReferencia=2024-01-15
```

### 📈 **Telemetria e Métricas**

```http
GET /telemetria/por-dia?dataReferencia=2024-01-15
```

## 🏗️ **Arquitetura**

```
├── 🎮 Hackathon.API          # Camada de apresentação (Controllers, Middleware)
├── 🧠 Hackathon.Application  # Casos de uso, Services, DTOs
├── 🏛️ Hackathon.Domain       # Regras de negócio, Entities, Value Objects
├── 🔧 Hackathon.Infrastructure # Persistência, Cache, EventHub
└── 🧪 *.Tests               # Testes unitários e de integração
```

### 🎯 **Fluxo da Simulação**

1. **Entrada**: Valor desejado + prazo em meses
2. **Validação**: Regras de negócio e constraints de produto
3. **Cálculo**: Algoritmos SAC e PRICE com precisão financeira
4. **Persistência**: Salva simulação no SQLite local
5. **Analytics**: Envia evento para Azure Event Hub
6. **Telemetria**: Registra métricas de performance automaticamente

## 🚀 **Diferenciais de Performance**

### ⚡ **Cache Inteligente**
- **Produtos em cache**: Primeira consulta carrega todos os produtos em memória
- **Zero latência**: Consultas subsequentes são instantâneas
- **Warm-up automático**: API inicializa com cache pré-carregado

### 📊 **Telemetria Avançada**
- **Coleta automática**: Métricas de tempo de resposta de todos os endpoints
- **Fire-and-forget**: Zero impacto na performance da API
- **Relatórios**: Dashboard de performance por data

### 🏗️ **Arquitetura Enterprise**
- **Clean Architecture**: Separação clara de responsabilidades
- **SOLID Principles**: Código extensível e testável
- **Domain-Driven Design**: Modelagem rica do domínio financeiro

## 🧪 **Executar Testes**

```bash
# Todos os testes
dotnet test

# Testes de um projeto específico
dotnet test Hackathon.Domain.Tests

# Com cobertura (se dotCover estiver configurado)
dotnet test --collect:"Code Coverage"
```

## 🐳 **Comandos Docker Úteis**

```bash
# Status dos containers
./scripts/docker-compose.sh status

# Limpeza completa (remove tudo)
./scripts/docker-compose.sh clean

# Logs em tempo real
docker-compose logs -f hackathon-api
```

## 📊 **Monitoramento**

### Health Check
```http
GET /health
```

### Métricas de Performance
A API coleta automaticamente:
- ⏱️ Tempo de resposta por endpoint
- 📈 Volume de requisições por data
- ✅ Taxa de sucesso/erro
- 🔍 Detalhamento por controller

## 🔧 **Configuração**

### **Banco de Dados**
- **Local**: SQLite (`data/hack.db`) para simulações e métricas
- **Externo**: SQL Server para catálogo de produtos

### **Cache**
- **MemoryCache**: Produtos financeiros (limite: 50 itens)
- **Estratégia**: Cache-aside com fallback para base

### **Observabilidade**
- **Logs estruturados**: Serilog com níveis configuráveis
- **Telemetria**: Coleta automática de métricas
- **Health checks**: Monitoramento de dependências

## 🎯 **Funcionalidades Avançadas**

### 💰 **Simulação Inteligente**
- **Validação automática**: Verifica se produto atende aos critérios
- **Cálculos precisos**: Arredondamento financeiro adequado
- **Múltiplos sistemas**: SAC (amortização constante) e PRICE (prestação fixa)

### 📈 **Analytics Empresarial**
- **Volume por produto**: Agregação automática por data
- **Métricas de uso**: Telemetria detalhada de performance
- **Relatórios**: APIs para dashboards e relatórios gerenciais

### 🛡️ **Robustez**
- **Exception handling**: Tratamento centralizado com respostas estruturadas
- **Validações**: FluentValidation para entrada de dados
- **Resiliência**: Retry policies e timeouts configurados

## 📚 **API Endpoints**

| Método | Endpoint | Descrição |
|--------|----------|-----------|
| `POST` | `/simulacao` | Realiza simulação de crédito |
| `GET` | `/simulacao` | Lista simulações (paginado) |
| `GET` | `/simulacao/volume-por-dia` | Volume simulado por produto/data |
| `GET` | `/telemetria/por-dia` | Métricas de performance por data |
| `GET` | `/health` | Health check da aplicação |

## 🎉 **Ready for Production**

✅ **Containerização otimizada** com multi-stage build
✅ **Usuário não-root** para segurança
✅ **Health checks** automáticos
✅ **Resource limits** configurados
✅ **Logs estruturados** para observabilidade
✅ **Variáveis de ambiente** para diferentes ambientes
✅ **Warm-up service** para eliminar cold start

---

**🏆 Esta API foi desenvolvida seguindo as melhores práticas de desenvolvimento .NET, com foco em performance, escalabilidade e manutenibilidade!**