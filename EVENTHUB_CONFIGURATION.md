# Configuração do EventHub

## Visão Geral

O EventHub é um serviço opcional que envia dados de simulação para o Azure Event Hubs. A aplicação foi projetada para funcionar mesmo quando o EventHub não está disponível ou configurado incorretamente.

## Configuração

### Habilitar EventHub

Para habilitar o EventHub, adicione a connection string no arquivo `appsettings.json` ou `appsettings.Development.json`:

```json
{
  "ConnectionStrings": {
    "ProdutosDb": "...",
    "LocalDb": "...",
    "EventHub": "Endpoint=sb://seu-eventhub.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=sua-chave;EntityPath=seu-eventhub"
  }
}
```

### Desabilitar EventHub

Para desabilitar o EventHub, simplesmente remova ou comente a linha da connection string:

```json
{
  "ConnectionStrings": {
    "ProdutosDb": "...",
    "LocalDb": "..."
    // "EventHub": "..." // Comentado para desabilitar
  }
}
```

## Comportamento

### Quando EventHub está habilitado e funcionando:
- As simulações são enviadas para o EventHub em background
- A resposta da API é retornada independentemente do sucesso do envio
- Logs informativos são gerados para acompanhar o envio

### Quando EventHub está desabilitado ou com erro:
- A aplicação continua funcionando normalmente
- As simulações são persistidas no banco de dados
- Logs de warning são gerados, mas não afetam o fluxo principal
- A resposta da API é retornada normalmente

## Logs

Os logs do EventHub podem ser configurados no `appsettings.json`:

```json
{
  "Logging": {
    "LogLevel": {
      "Hackathon.Infrastructure.EventHub.EventHubService": "Information"
    }
  }
}
```

## Troubleshooting

### Problemas Comuns

1. **Connection string inválida**: A aplicação não crasha, apenas desabilita o EventHub
2. **EventHub indisponível**: As simulações continuam sendo processadas normalmente
3. **Timeout de conexão**: O envio é ignorado e logs de warning são gerados

### Verificação

Para verificar se o EventHub está funcionando, observe os logs:
- `"=== EVENTHUB SERVICE INICIALIZADO COM SUCESSO ==="` - EventHub habilitado
- `"EventHub connection string não configurada! EventHub será desabilitado."` - EventHub desabilitado
- `"Simulação enviada para EventHub"` - Envio bem-sucedido
- `"Falha ao enviar simulação para EventHub"` - Erro no envio (não afeta a resposta)
