# 🏦 API Simulador de Crédito - CAIXA Hackathon

## 📋 Descrição

Sistema de simulação de crédito desenvolvido para o Hackathon da CAIXA, permitindo que qualquer pessoa ou sistema descubra as condições oferecidas para negociação de empréstimos. A solução implementa os sistemas de amortização SAC e PRICE, integra com Azure Event Hub para estratégias de relacionamento com o cliente e oferece telemetria completa dos serviços.

## 🎯 Objetivo

Disponibilizar para todos os brasileiros a possibilidade de simulação de empréstimo através de uma API robusta e escalável, integrando tecnologias modernas como .NET 8, Azure Event Hub e SQL Server.

## 🏗️ Arquitetura

O projeto segue a arquitetura **Clean Architecture** com separação clara de responsabilidades:

```
📁 Hackathon/
├── 🏛️ Hackathon.Domain/          # Entidades e regras de negócio
├── 🎯 Hackathon.Application/      # Casos de uso e handlers
├── 🌐 Hackathon.API/             # Controllers e contratos da API
├── 🗄️ Hackathon.Infrastructure/  # Implementações de repositórios e serviços
└── 🧪 **.Tests/                  # Testes unitários e de integração
```

## 🚀 Funcionalidades

### ✅ Implementadas
- ✅ **Simulação de Crédito**: Cálculos SAC e PRICE
- ✅ **Validação de Produtos**: Baseada em parâmetros do banco
- ✅ **Persistência Local**: SQLite para histórico de simulações
- ✅ **Integração Event Hub**: Para estratégias de relacionamento
- ✅ **Telemetria**: Monitoramento de performance e volumes
- ✅ **Listagem de Simulações**: Com paginação
- ✅ **Volume por Dia**: Relatórios de volume simulado
- ✅ **Containerização**: Docker e Docker Compose
- ✅ **Testes**: Cobertura completa com dotCover

### 📊 Endpoints Disponíveis

| Método | Endpoint | Descrição |
|--------|----------|-----------|
| `POST` | `/simulacao` | Realizar simulação de crédito |
| `GET` | `/simulacao` | Listar simulações com paginação |
| `GET` | `/simulacao/volume-por-dia` | Volume simulado por produto/dia |
| `GET` | `/telemetria` | Dados de telemetria dos serviços |

## 🛠️ Tecnologias Utilizadas

- **.NET 8** - Framework principal
- **Entity Framework Core** - ORM para acesso a dados
- **MediatR** - Padrão Mediator para handlers
- **Mapster** - Mapeamento de objetos
- **FluentValidation** - Validações
- **SQL Server** - Banco de dados principal (Azure)
- **SQLite** - Banco local para simulações
- **Azure Event Hub** - Integração de eventos
- **Docker** - Containerização
- **Swagger** - Documentação da API
- **xUnit** - Framework de testes
- **dotCover** - Cobertura de código

## 📦 Pré-requisitos

- .NET 8 SDK
- Docker e Docker Compose
- SQL Server (opcional para desenvolvimento local)

## 🚀 Como Executar


### 1. Execução com Docker (Recomendado)

#### Desenvolvimento
```bash
docker-compose --profile dev up --build
```

#### Produção
```bash
docker-compose --profile prod up --build
```

### 2. Execução Local

```bash
# Restaurar dependências
dotnet restore

# Executar a API
cd Hackathon.API
dotnet run

# Executar testes
dotnet test
```

## 🌐 Acessos

- **API**: http://localhost:5000 (dev) ou http://localhost:8080 (prod)
- **Swagger**: http://localhost:5000/swagger (dev)
- **Health Check**: http://localhost:5000/health

## 📝 Exemplos de Uso

### Realizar Simulação
```bash
curl -X POST "http://localhost:5000/simulacao" \
  -H "Content-Type: application/json" \
  -d '{
    "valorDesejado": 900.00,
    "prazo": 5
  }'
```

### Listar Simulações
```bash
curl -X GET "http://localhost:5000/simulacao?pagina=1&qtdRegistrosPagina=10"
```

### Obter Volume por Dia
```bash
curl -X GET "http://localhost:5000/simulacao/volume-por-dia?dataReferencia=2025-01-30"
```

### Obter Telemetria
```bash
curl -X GET "http://localhost:5000/telemetria?dataReferencia=2025-01-30"
```

## 🧪 Testes

### Executar Todos os Testes
```bash
dotnet test
```


### Executar Testes Específicos
```bash
# Testes da API
dotnet test Hackathon.API.Tests/

# Testes da Application
dotnet test Hackathon.Application.Tests/

# Testes do Domain
dotnet test Hackathon.Domain.Tests/

# Testes da Infrastructure
dotnet test Hackathon.Infrastructure.Tests/
```




### Sistemas de Amortização

#### SAC (Sistema de Amortização Constante)
- Amortização constante
- Juros decrescentes
- Prestações decrescentes

#### PRICE (Sistema Francês)
- Prestações constantes
- Amortização crescente
- Juros decrescentes

## 🔧 Configurações

O projeto utiliza configurações via `appsettings.json` e variáveis de ambiente para diferentes ambientes (Development e Production). As configurações incluem conexões com bancos de dados, Event Hub e configurações da aplicação.

## 📈 Monitoramento

### Telemetria
O sistema coleta automaticamente:
- Quantidade de requisições por endpoint
- Tempo médio, mínimo e máximo de resposta
- Percentual de sucesso
- Volumes de simulação por produto

### Health Checks
- Verificação de conectividade com bancos de dados
- Status do Event Hub
- Disponibilidade dos serviços

## 🔄 Azure Event Hub

### Visão Geral
O sistema integra com **Azure Event Hub** para enviar dados de simulação em tempo real, permitindo que a área de relacionamento com o cliente receba eventos de simulação em poucos segundos e execute estratégias negociais baseadas na interação do cliente.

### Funcionalidades Implementadas

#### 📤 **Envio de Eventos**
- **Fire-and-Forget**: Eventos enviados de forma assíncrona sem bloquear a resposta da API
- **Serialização Otimizada**: JSON comprimido para melhor performance
- **Retry Policy**: Configuração de retry exponencial (máximo 3 tentativas)
- **Propriedades de Evento**: Metadados para facilitar filtros no consumidor

#### 🏗️ **Arquitetura de Integração**
```
┌─────────────────┐    ┌──────────────────┐    ┌─────────────────┐
│   API Request   │───▶│  SimulacaoResult │───▶│  EventPublisher │
└─────────────────┘    └──────────────────┘    └─────────────────┘
                                                         │
                                                         ▼
                                               ┌─────────────────┐
                                               │ EventHubService │
                                               └─────────────────┘
                                                         │
                                                         ▼
                                               ┌─────────────────┐
                                               │ Azure Event Hub │
                                               └─────────────────┘
```

#### 🔧 **Configurações Técnicas**
- **Connection Pooling**: Singleton para reutilizar conexões
- **Compressão**: Dados serializados de forma otimizada
- **Timeout**: Configuração de timeout para evitar travamentos
- **Logging**: Logs estruturados para auditoria e troubleshooting

#### 📊 **Dados Enviados**
Cada evento contém:
- **Dados da Simulação**: Resultado completo com cálculos SAC e PRICE
- **Metadados**: Timestamp, tipo de evento, origem
- **Propriedades**: EventType, Timestamp, Source

#### 🚀 **Benefícios para o Negócio**
- **Tempo Real**: Eventos recebidos em poucos segundos
- **Estratégias Personalizadas**: Baseadas no comportamento do cliente
- **Analytics**: Dados para análise de padrões de simulação
- **Integração**: Possibilidade de conectar com outros sistemas da CAIXA

#### 🧪 **Testes**
- **Testes Unitários**: Cobertura completa do EventHubService
- **Testes de Integração**: Validação do EventPublisher
- **Warmup**: Verificação de conectividade na inicialização
- **Error Handling**: Tratamento robusto de falhas

