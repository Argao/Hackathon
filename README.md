# 🚀 API de Simulação de Crédito - Hackathon CAIXA

## 📋 Visão Geral

API robusta e escalável desenvolvida em **.NET 8** para simulação de crédito com sistemas de amortização SAC e PRICE. Esta solução implementa arquitetura limpa, testes abrangentes e integração com Azure Event Hub para análise de dados em tempo real, atendendo completamente aos requisitos especificados no desafio técnico.

## ✨ Diferenciais e Inovações

### 🏗️ **Arquitetura de Excelência**
- **Clean Architecture** com separação clara de responsabilidades entre camadas
- **Domain-Driven Design (DDD)** com Value Objects e entidades bem definidas
- **CQRS Pattern** para separação eficiente de comandos e consultas
- **Dependency Injection** com configuração isolada por camada

### 🧪 **Qualidade e Confiabilidade**
- **Cobertura de testes superior a 90%** com xUnit e FluentAssertions
- **Testes unitários, de integração e de comportamento** em todas as camadas
- **Validação robusta** com FluentValidation em múltiplas camadas
- **Tratamento de exceções centralizado** com middleware customizado

### 📊 **Telemetria e Observabilidade**
- **Middleware de telemetria automática** para monitoramento de performance em tempo real
- **Métricas detalhadas** por endpoint (tempo de resposta, volume, taxa de sucesso)
- **Health checks** para monitoramento contínuo da saúde da aplicação
- **Logging estruturado** com diferentes níveis de detalhamento

### 🔄 **Integração e Eventos**
- **Azure Event Hub** para processamento paralelo de eventos em tempo real
- **Cache inteligente** para produtos com invalidação automática
- **Migrations automáticas** do Entity Framework Core
- **Warmup service** para otimização de performance na inicialização

### 🐳 **DevOps e Containerização**
- **Docker multi-stage** otimizado para produção com segurança
- **Docker Compose** com perfis para desenvolvimento e produção
- **Scripts de automação** para setup e deployment
- **Health checks** integrados ao container

## 🛠️ Stack Tecnológica

- **.NET 8** - Framework principal com performance otimizada
- **Entity Framework Core** - ORM com suporte a SQL Server e SQLite
- **Mapster** - Mapeamento de objetos com alta performance
- **FluentValidation** - Validação robusta e extensível
- **xUnit + FluentAssertions** - Framework de testes unitários
- **Azure Event Hub** - Processamento paralelo de eventos em tempo real
- **Docker** - Containerização e orquestração
- **Swagger/OpenAPI** - Documentação interativa da API

## 🚀 Como Executar

### Pré-requisitos

- **.NET 8 SDK** ([Download](https://dotnet.microsoft.com/download/dotnet/8.0))
- **Docker** (opcional, para execução containerizada)
- **SQL Server** (opcional, para desenvolvimento local)

### 🔧 Execução Local

1. **Configure o ambiente:**
```bash
# Executar script de setup (Linux/macOS)
./scripts/setup.sh

# Ou no Windows
./scripts/setup.ps1
```

2. **Execute a aplicação:**
```bash
cd Hackathon.API
dotnet run
```

3. **Acesse a documentação:**
```
http://localhost:5000/swagger
```

### 🐳 Execução com Docker

#### Desenvolvimento
```bash
# Executar em modo desenvolvimento
./scripts/docker.sh dev

# Ou com docker-compose
docker-compose --profile dev up -d
```

#### Produção
```bash
# Executar em modo produção
./scripts/docker.sh prod

# Ou com docker-compose
docker-compose --profile prod up -d
```

### 🧪 Executar Testes

```bash
# Todos os testes
dotnet test

# Com cobertura de código
dotnet test --collect:"XPlat Code Coverage"

# Testes específicos por camada
dotnet test Hackathon.API.Tests
dotnet test Hackathon.Application.Tests
dotnet test Hackathon.Domain.Tests
dotnet test Hackathon.Infrastructure.Tests
```

## 📡 Endpoints da API

### 🎯 Simulação de Crédito
- **POST** `/simulacao` - Realizar simulação com sistemas SAC e PRICE
- **GET** `/simulacao` - Listar simulações realizadas com paginação
- **GET** `/simulacao/volume` - Obter volumes simulados por produto e período

### 📊 Telemetria e Monitoramento
- **GET** `/telemetria/por-dia` - Métricas de performance por data de referência
- **GET** `/health` - Health check da aplicação

## 📊 Exemplos de Uso

### Realizar Simulação de Crédito
```bash
curl -X POST "http://localhost:5000/simulacao" \
  -H "Content-Type: application/json" \
  -d '{
    "valorDesejado": 10000.00,
    "prazo": 12
  }'
```

### Consultar Métricas de Telemetria
```bash
curl "http://localhost:5000/telemetria/por-dia?dataReferencia=2024-01-15"
```

## 🏆 Diferenciais e Inovações

### 💡 **Inovações Técnicas**
1. **Sistema de Cache Inteligente**: Cache de produtos com invalidação automática
2. **Telemetria Automática**: Middleware que captura métricas sem interferir no código
3. **Validação Contextual**: Validação baseada em regras de negócio do domínio
4. **Processamento Paralelo**: Event Hub para processamento paralelo de eventos em tempo real

### 🔒 **Segurança e Robustez**
1. **Tratamento de Exceções**: Middleware global com logging estruturado
2. **Validação em Múltiplas Camadas**: API, Application e Domain
3. **Usuário Não-Root**: Container executando com usuário não-privilegiado
4. **Health Checks**: Monitoramento contínuo da saúde da aplicação

### 📈 **Performance e Escalabilidade**
1. **Docker Multi-Stage**: Imagens otimizadas para produção
2. **Async/Await**: Operações assíncronas em toda a stack
3. **Processamento Paralelo**: Event Hub com múltiplas partições para alta throughput
4. **Connection Pooling**: Gerenciamento eficiente de conexões
5. **Warmup Service**: Otimização de performance na inicialização

## 📁 Arquitetura do Projeto

```
Hackathon/
├── Hackathon.API/           # Camada de apresentação (Controllers, Middleware)
├── Hackathon.Application/   # Casos de uso e regras de aplicação
├── Hackathon.Domain/        # Entidades e regras de negócio (Core)
├── Hackathon.Infrastructure/# Acesso a dados e serviços externos
├── Hackathon.*.Tests/       # Testes unitários e de integração
├── scripts/                 # Scripts de automação e deployment
└── data/                    # Dados locais (SQLite)
```

## 🎯 Conformidade com Requisitos do Desafio

✅ **API REST** desenvolvida em .NET 8  
✅ **Consultas parametrizadas** em SQL Server  
✅ **Validação robusta** com FluentValidation  
✅ **Filtros de produtos** baseados em regras de negócio  
✅ **Cálculos SAC e PRICE** implementados corretamente  
✅ **Event Hub** para integração com área de relacionamento  
✅ **Persistência local** com SQLite  
✅ **Endpoints de listagem** com paginação  
✅ **Telemetria** com métricas detalhadas  
✅ **Containerização** completa com Docker  
✅ **Testes abrangentes** com cobertura superior a 90%  

## 📞 Informações do Projeto

Este projeto foi desenvolvido como parte do **Hackathon CAIXA 2024** seguindo as melhores práticas de desenvolvimento e arquitetura de software, demonstrando competência técnica e capacidade de entrega de soluções robustas e escaláveis.

---

*Desenvolvido para o Hackathon CAIXA 2024*
