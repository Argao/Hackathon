# API de Simulação de Crédito - Hackathon CAIXA

[![.NET 8.0](https://img.shields.io/badge/.NET-8.0-blue.svg)](https://dotnet.microsoft.com/download/dotnet/8.0)
[![Docker](https://img.shields.io/badge/Docker-Ready-blue.svg)](https://www.docker.com/)
[![Swagger](https://img.shields.io/badge/Swagger-Documentation-green.svg)](http://localhost:5000/swagger)
[![Health Check](https://img.shields.io/badge/Health%20Check-Available-green.svg)](http://localhost:5000/health)

## 📋 Índice

- [🎯 O Problema](#-o-problema-que-resolve)
- [💡 Solução Proposta](#-solução-proposta)
- [📋 Pré-requisitos](#-pré-requisitos)
- [🔧 Configuração](#-configuração)
- [🚀 Execução Rápida](#-execução-rápida)
- [🏗️ Arquitetura](#️-arquitetura)
- [📡 Endpoints da API](#-endpoints-da-api)
- [🗄️ Banco de Dados](#️-banco-de-dados)
- [📡 Integração EventHub](#-integração-eventhub)
- [🧪 Testes](#-testes)
- [📊 Health Checks](#-health-checks)
- [📈 Impacto Real](#-impacto-real)
- [🎯 Próximos Passos](#-próximos-passos)

## 🎯 O Problema que Resolve

Imagine que você é um microempreendedor como o Lucas, que precisa investir em novos equipamentos para sua pequena empresa. Você quer saber exatamente quanto vai pagar por mês, mas enfrenta alguns desafios:

- **Não tem tempo** para ir até uma agência bancária
- **Não entende** as diferenças entre SAC e PRICE
- **Quer comparar** diferentes prazos e valores
- **Precisa de transparência** total nos custos
- **Deseja tomar decisões informadas** sobre seu crédito

Este é o cenário que **milhões de brasileiros** enfrentam diariamente. A CAIXA, como banco público, tem a missão de democratizar o acesso ao crédito e tornar as informações financeiras mais transparentes e acessíveis para todos.

## 💡 Solução Proposta

A solução é uma **API inteligente** que permite que **qualquer pessoa ou sistema** descubra as condições de empréstimo de forma simples, rápida e transparente. É como ter um consultor financeiro 24 horas por dia, disponível para todos os brasileiros.

### O que a API faz:

1. **Recebe uma solicitação simples**: valor desejado e prazo
2. **Consulta produtos disponíveis**: automaticamente encontra o produto adequado
3. **Calcula múltiplas opções**: SAC (amortização constante) e PRICE (prestação fixa)
4. **Mostra tudo transparentemente**: valor da parcela, juros, amortização mês a mês
5. **Envia para área comercial**: Eventos enviados para Azure EventHub em tempo real
6. **Mantém histórico**: para análise e melhorias futuras

## 📋 Pré-requisitos

### Requisitos do Sistema
- **.NET 8.0 SDK** (versão exata: 8.0.0)
- **Docker** e **Docker Compose**
- **SQL Server** (Azure SQL Database configurado)
- **Azure EventHub** (configurado)

### Ferramentas Recomendadas
- **Visual Studio 2022** ou **Rider**
- **Postman** ou **Insomnia** para testes da API
- **Azure Data Studio** ou **SQL Server Management Studio**

## 🔧 Configuração

### Variáveis de Ambiente Necessárias

```bash
# Banco de Dados SQL Server (Azure)
ConnectionStrings__ProdutosDb=Server=dbhackathon.database.windows.net,1433;Database=hack;User ID=hack;Password=Password23;Encrypt=True;TrustServerCertificate=True;MultipleActiveResultSets=True;Connection Timeout=30;

# Banco Local SQLite (para desenvolvimento)
ConnectionStrings__LocalDb=Data Source=./data/hack.db

# Azure EventHub
ConnectionStrings__EventHub=Endpoint=sb://eventhack.servicebus.windows.net/;SharedAccessKeyName=hack;SharedAccessKey=HeHeVaVqyVkntO2FnjQcs2Ilh/4MUDo4y+AEhKp8z+g=;EntityPath=simulacoes
```

### Configuração do Ambiente

As variáveis de ambiente já estão configuradas no `docker-compose.yml` e `appsettings.json`. Para execução local, certifique-se de que o arquivo `appsettings.Development.json` existe com as configurações adequadas.

## 🚀 Execução Rápida

### Opção 1: Usando Scripts (Recomendado)

```bash
# Desenvolvimento
./scripts/docker.sh dev

# Produção
./scripts/docker.sh prod

# Parar serviços
./scripts/docker.sh stop
```

### Opção 2: Usando Docker Compose

```bash
# Desenvolvimento
docker-compose --profile dev up --build

# Produção
docker-compose --profile prod up --build
```

### Opção 3: Execução Local

```bash
# Pré-requisitos
- .NET 8.0 SDK

# Executar
cd Hackathon.API
dotnet run
```

### Acessar Documentação Interativa

```
http://localhost:5000/swagger
```

A documentação Swagger permite testar todos os endpoints diretamente no navegador.

## 🏗️ Arquitetura

### Estrutura de Camadas
- **Hackathon.API**: Controllers, Middleware, Mappings
- **Hackathon.Application**: Commands, Queries, Handlers, Services
- **Hackathon.Domain**: Entities, ValueObjects, Interfaces
- **Hackathon.Infrastructure**: Context, Repositories, EventHub

### Padrões Utilizados
- **CQRS** com MediatR
- **Clean Architecture**
- **Repository Pattern**
- **Value Objects**
- **Domain-Driven Design**

### Benefícios da Arquitetura
- **Separação de responsabilidades**: Cada parte do sistema tem uma função específica
- **Testabilidade**: Fácil de testar cada camada isoladamente
- **Manutenibilidade**: Código organizado e bem estruturado
- **Escalabilidade**: Preparado para crescimento futuro

## 📡 Endpoints da API

### Simulação
```http
POST /simulacao
```
Realiza simulação completa com múltiplos métodos de amortização (SAC e PRICE)

**Exemplo de Request:**
```json
{
    "valorDesejado": 5000.00,
    "prazo": 12
}
```

**Exemplo de Response:**
```json
{
    "idSimulacao": 20250130001,
    "codigoProduto": 2,
    "descricaoProduto": "Produto 2",
    "taxaJuros": 0.0175,
    "resultadoSimulacao": [
        {
            "tipo": "SAC",
            "parcelas": [
                {
                    "numero": 1,
                    "valorAmortizacao": 416.67,
                    "valorJuros": 87.50,
                    "valorPrestacao": 504.17
                }
            ]
        },
        {
            "tipo": "PRICE",
            "parcelas": [
                {
                    "numero": 1,
                    "valorAmortizacao": 400.00,
                    "valorJuros": 87.50,
                    "valorPrestacao": 487.50
                }
            ]
        }
    ]
}
```

### Histórico de Simulações
```http
GET /simulacao?pagina=1&qtdRegistros=10
```
Lista simulações realizadas com paginação

### Volume por Dia
```http
GET /simulacao/volume-por-dia?dataReferencia=2025-01-30
```
Relatórios de volume simulado por produto

### Telemetria
```http
GET /telemetria/por-dia?dataReferencia=2025-01-30
```
Métricas de performance e uso da API

### Health Checks
```http
GET /health
GET /telemetria/health
```
Verificação de saúde dos serviços

## 🗄️ Banco de Dados

### Migrações Disponíveis
- **19 migrações** implementadas
- Schema otimizado com índices de performance
- Suporte a SQL Server (Azure) e SQLite (local)

### Executar Migrações

```bash
# Desenvolvimento (SQLite)
dotnet ef database update --project Hackathon.Infrastructure

# Produção (SQL Server)
dotnet ef database update --project Hackathon.Infrastructure --connection "Server=dbhackathon.database.windows.net,1433;Database=hack;User ID=hack;Password=Password23;Encrypt=True;TrustServerCertificate=True;MultipleActiveResultSets=True;Connection Timeout=30;"
```

### Estrutura das Tabelas

#### Tabela PRODUTO (SQL Server Azure)
```sql
CREATE TABLE dbo.PRODUTO (
    CO_PRODUTO int NOT NULL primary key,
    NO_PRODUTO varchar(200) NOT NULL,
    PC_TAXA_JUROS numeric(10, 9) NOT NULL,
    NU_MINIMO_MESES smallint NOT NULL,
    NU_MAXIMO_MESES smallint NULL,
    VR_MINIMO numeric(18, 2) NOT NULL,
    VR_MAXIMO numeric(18, 2) NULL
);
```

#### Tabelas Locais (SQLite)
- **SIMULACAO**: Dados das simulações realizadas
- **RESULTADO_SIMULACAO**: Resultados detalhados
- **PARCELA**: Parcelas de cada simulação
- **METRICA_REQUISICAO**: Métricas de telemetria

## 📡 Integração EventHub

### Funcionalidades
- **Envio assíncrono** de simulações
- **Retry automático** com backoff exponencial
- **Compressão de dados** para performance
- **Logs detalhados** de eventos

### Configuração
- **Endpoint**: `sb://eventhack.servicebus.windows.net/`
- **Entity**: `simulacoes`
- **Shared Access Key** configurada

### Eventos Enviados
```json
{
    "EventType": "SimulacaoRealizada",
    "Timestamp": "2025-01-30T10:30:00Z",
    "Source": "HackathonAPI",
    "Data": {
        "idSimulacao": 20250130001,
        "valorDesejado": 5000.00,
        "prazo": 12
    }
}
```

## 🧪 Testes

### Executar Todos os Testes
```bash
dotnet test
```

### Executar Testes por Camada
```bash
dotnet test Hackathon.API.Tests
dotnet test Hackathon.Application.Tests
dotnet test Hackathon.Domain.Tests
dotnet test Hackathon.Infrastructure.Tests
```

### Cobertura de Código
- **Coverlet** configurado para cobertura
- **FluentAssertions** para assertions
- **Moq** para mocking
- **xUnit** como framework de testes

### Estrutura de Testes
- **Testes Unitários**: Cada camada isoladamente
- **Testes de Integração**: API e banco de dados
- **Testes de Comportamento**: Cenários de negócio

## 📊 Health Checks

### Endpoints Disponíveis
- `GET /health` - Health check geral da aplicação
- `GET /telemetria/health` - Health check específico da telemetria

### Configuração Docker
- Health checks configurados no docker-compose
- **Intervalo**: 30s
- **Timeout**: 10s
- **Retries**: 3

### Exemplo de Response
```json
{
    "service": "Telemetria",
    "status": "healthy",
    "timestamp": "2025-01-30T10:30:00Z",
    "version": "1.0.0"
}
```

## 📈 Impacto Real

### Para o Cliente
- **Transparência total**: Sabe exatamente quanto vai pagar de juros e amortização
- **Comparação fácil**: Vê múltiplas opções de amortização lado a lado
- **Decisão informada**: Escolhe a melhor opção para seu perfil financeiro
- **Economia de tempo**: Não precisa ir até agência ou aguardar atendimento
- **Confiança**: Entende completamente o que está contratando

### Para a CAIXA
- **Atendimento eficiente**: Clientes já chegam informados e preparados
- **Oportunidades comerciais**: Recebe dados em tempo real via EventHub
- **Redução de custos**: Processo automatizado reduz necessidade de atendimento manual
- **Inclusão financeira**: Democratiza acesso ao crédito para todos os brasileiros
- **Dados valiosos**: Histórico de simulações para análise de comportamento

### Para o Sistema Financeiro
- **Padrão de transparência**: Mostra como APIs bancárias devem funcionar
- **Integração fácil**: Outros sistemas podem usar a API sem dificuldades
- **Dados estruturados**: Informações organizadas para análise e tomada de decisão
- **Inovação**: Abre caminho para novos produtos e serviços digitais

## 🎯 Próximos Passos

Esta API é apenas o começo de uma transformação digital maior. Com ela, podemos:

1. **Integrar com apps mobile**: Para acesso ainda mais fácil e intuitivo
2. **Adicionar mais produtos**: Cartão de crédito, financiamento imobiliário, consignado
3. **Personalizar ofertas**: Baseado no histórico e perfil do cliente
4. **Expandir para outros bancos**: Criar um padrão do setor financeiro
5. **Machine Learning**: Sugerir produtos baseado no comportamento do usuário
6. **Integração com PIX**: Para pagamentos e transferências
7. **Chatbot inteligente**: Para tirar dúvidas sobre simulações
8. **Expansão do EventHub**: Integração com mais sistemas de relacionamento e CRM

## 🌟 Diferenciais da Solução

### Transparência Total
- Todos os cálculos são visíveis e compreensíveis
- Comparação lado a lado entre múltiplos métodos de amortização
- Detalhamento mês a mês de juros e amortização
- Arquitetura extensível para novos métodos de cálculo

### Acessibilidade
- API disponível 24h por dia
- Documentação interativa e clara
- Resposta rápida e confiável

### Integração
- Eventos enviados para área comercial via Azure EventHub em tempo real
- Histórico completo de simulações
- Métricas de uso e performance

### Qualidade e Confiabilidade
- Cobertura abrangente de testes em todas as camadas
- Validação robusta de dados de entrada
- Tratamento de exceções centralizado

### Escalabilidade
- Arquitetura preparada para alto volume
- Cache inteligente para performance
- Processamento assíncrono de eventos

## 💬 Conclusão

A solução proposta transforma a experiência de simulação de crédito de um processo burocrático, demorado e confuso em uma experiência digital rápida, transparente e educativa. 

É um exemplo de como a tecnologia pode democratizar o acesso a serviços financeiros, contribuindo para a **inclusão financeira** no Brasil e tornando o sistema bancário mais acessível e transparente para todos.

A API proposta não é apenas um código - é uma **ferramenta de transformação social**, que coloca o poder da informação financeira nas mãos de todos os brasileiros, independentemente de onde estejam ou de quanto tempo tenham disponível.

---

**Desenvolvido para o Hackathon CAIXA**  
*Democratizando o acesso ao crédito através da tecnologia*
