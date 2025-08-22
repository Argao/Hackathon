# Configuração de Logs - Hackathon API

## Visão Geral

As configurações de logging foram reorganizadas de forma mais lógica e estruturada, priorizando a visibilidade dos logs principais da aplicação e reduzindo o ruído do Entity Framework.

## Estrutura de Logs

### 🔴 Logs Críticos (Error)
- **Erros de aplicação**: Falhas críticas que impedem o funcionamento
- **Erros de persistência**: Falhas na gravação de dados
- **Erros de integração**: Falhas no EventHub e serviços externos

### 🟡 Logs de Aviso (Warning)
- **Validações falhadas**: Dados inválidos recebidos
- **Produtos não encontrados**: Simulações sem produtos adequados
- **Falhas não críticas**: EventHub em background, warm-up parcial

### 🟢 Logs Informativos (Information)
- **Operações principais**: Início e fim de simulações
- **Persistência bem-sucedida**: Gravação de dados
- **Cache**: Hit/miss e preenchimento
- **EventHub**: Envio bem-sucedido
- **Warm-up**: Inicialização da aplicação

### 🔵 Logs de Debug (Debug)
- **Detalhes de operações**: Validações, cálculos, mapeamentos
- **Telemetria**: Métricas de performance
- **Repositórios**: Operações de banco de dados
- **Middleware**: Interceptação de requisições

## Configurações por Camada

### Application Layer (Information)
- `Hackathon.Application.Services.SimulacaoService` - Serviço principal de simulações
- `Hackathon.Application.Services.TelemetriaService` - Coleta de métricas
- `Hackathon.Application.Services.CachedProdutoService` - Cache de produtos

### Infrastructure Layer
- `Hackathon.Infrastructure.EventHub.EventHubService` - Integração EventHub (Information)
- `Hackathon.Infrastructure.Services.WarmupService` - Inicialização da aplicação (Information)
- `Hackathon.Infrastructure.Repositories.MetricaRepository` - Persistência de métricas (Debug)

### API Layer
- `Hackathon.API.Middleware.TelemetriaMiddleware` - Interceptação de requisições (Debug)

### Framework Logs (Warning)
- `Microsoft.EntityFrameworkCore` - Logs do Entity Framework reduzidos
- `Microsoft.AspNetCore` - Logs do ASP.NET Core reduzidos

## Arquivos de Configuração

### appsettings.json
Configuração padrão para produção com logs otimizados.

### appsettings.Development.json
Configuração para desenvolvimento com logs otimizados.

### appsettings.Development.Detailed.json
Configuração opcional para desenvolvimento com logs detalhados do EF.

## Hierarquia de Logs

```
ERROR   - Falhas críticas que impedem operação
WARNING - Problemas que não impedem funcionamento
INFO    - Operações principais e resultados
DEBUG   - Detalhes técnicos para troubleshooting
TRACE   - Informações muito detalhadas (não configurado)
```

## Benefícios da Nova Estrutura

1. **Visibilidade clara**: Logs principais em destaque
2. **Redução de ruído**: Entity Framework silenciado
3. **Hierarquia lógica**: Logs organizados por importância
4. **Debug facilitado**: Informações técnicas quando necessário
5. **Performance**: Menos overhead de logging em produção

## Exemplos de Logs

### Simulação de Crédito
```
INFO  - Iniciando simulação de crédito - Valor: 50000, Prazo: 12 meses
INFO  - Produto selecionado: PESSOAL - Empréstimo Pessoal
INFO  - Persistindo simulação ID: 12345
INFO  - Simulação persistida com sucesso - ID: 12345, Duração: 45ms
INFO  - Dados enviados para EventHub com sucesso - Tamanho: 1024 bytes
INFO  - Simulação processada com sucesso - ID: 12345
```

### Cache de Produtos
```
INFO  - Cache miss: Buscando produtos do banco de dados
INFO  - Cache preenchido: 4 produtos por 240 minutos
DEBUG - Cache hit: 4 produtos obtidos da memória
```

### Warm-up da Aplicação
```
INFO  - Iniciando warm-up da aplicação
INFO  - Pré-carregando produtos em cache
INFO  - Cache otimizado: 4 produtos carregados em 26ms
INFO  - Warm-up concluído em 1503ms
```
