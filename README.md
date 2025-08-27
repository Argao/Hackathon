# API de Simulação de Crédito - Hackathon CAIXA

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
3. **Calcula múltiplas opções**: Atualmente SAC (amortização constante) e PRICE (prestação fixa), com arquitetura preparada para futuros métodos de amortização
4. **Mostra tudo transparentemente**: valor da parcela, juros, amortização mês a mês
5. **Envia para área comercial**: Eventos enviados para Azure EventHub em tempo real
6. **Mantém histórico**: para análise e melhorias futuras

## 🚀 Como Funciona na Prática

### Para o Cliente Final

**Antes**: 
- Ir até agência bancária
- Aguardar atendimento (às vezes horas)
- Receber apenas uma opção
- Não entender os cálculos
- Tomar decisões sem informação completa

**Agora**:
- Acessar de qualquer lugar, 24h por dia
- Receber resposta de forma ágil
- Ver múltiplas opções de pagamento lado a lado (SAC e PRICE)
- Entender exatamente quanto paga de juros
- Tomar decisões informadas e confiantes

### Para a CAIXA

**Antes**:
- Clientes chegavam sem informação prévia
- Processo manual demorado e propenso a erros
- Dificuldade para oferecer produtos adequados
- Perda de oportunidades comerciais

**Agora**:
- Clientes já chegam informados e preparados
- Processo automatizado, rápido e preciso
- Área comercial recebe dados em tempo real para estratégias
- Oportunidades de negócio identificadas automaticamente

## 📱 Como Usar

### Simulação Básica

```json
POST /simulacao
{
    "valorDesejado": 5000.00,
    "prazo": 12
}
```

### Resposta Completa e Transparente

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
                },
                {
                    "numero": 2,
                    "valorAmortizacao": 416.67,
                    "valorJuros": 79.17,
                    "valorPrestacao": 495.84
                }
                // ... mais parcelas com detalhamento completo
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
                },
                {
                    "numero": 2,
                    "valorAmortizacao": 406.00,
                    "valorJuros": 81.50,
                    "valorPrestacao": 487.50
                }
                // ... mais parcelas com prestação fixa
            ]
        }
    ]
}
```

## 🏗️ Arquitetura Simples e Robusta

### Por que esta abordagem foi escolhida:

1. **Separação de responsabilidades**: Cada parte do sistema tem uma função específica e bem definida
2. **Cache inteligente**: Produtos são consultados de forma ágil, sem demora para o usuário
3. **Processamento assíncrono**: Eventos são enviados para Azure EventHub sem atrasar a resposta ao cliente
4. **Monitoramento automático**: Sabemos sempre como o sistema está funcionando
5. **Escalabilidade**: Pode atender milhões de usuários simultaneamente

### Benefícios para o usuário:

- **Resposta rápida**: Simulações processadas de forma ágil
- **Sempre disponível**: Sistema funciona 24h por dia, 7 dias por semana
- **Escalável**: Atende milhões de usuários simultaneamente
- **Confiável**: Falhas são tratadas automaticamente
- **Transparente**: Todos os cálculos são visíveis e compreensíveis

## 📊 Impacto Real

### Para o Cliente
- **Transparência total**: Sabe exatamente quanto vai pagar de juros e amortização
- **Comparação fácil**: Vê múltiplas opções de amortização lado a lado (SAC e PRICE) com detalhamento completo
- **Decisão informada**: Escolhe a melhor opção para seu perfil financeiro
- **Economia de tempo**: Não precisa ir até agência ou aguardar atendimento
- **Confiança**: Entende completamente o que está contratando

### Para a CAIXA
- **Atendimento eficiente**: Clientes já chegam informados e preparados
- **Oportunidades comerciais**: Recebe dados em tempo real via EventHub para estratégias
- **Redução de custos**: Processo automatizado reduz necessidade de atendimento manual
- **Inclusão financeira**: Democratiza acesso ao crédito para todos os brasileiros
- **Dados valiosos**: Histórico de simulações para análise de comportamento

### Para o Sistema Financeiro
- **Padrão de transparência**: Mostra como APIs bancárias devem funcionar
- **Integração fácil**: Outros sistemas podem usar a API sem dificuldades
- **Dados estruturados**: Informações organizadas para análise e tomada de decisão
- **Inovação**: Abre caminho para novos produtos e serviços digitais
- **Integração com EventHub**: Permite integração com sistemas de relacionamento e CRM

## 🛠️ Como Executar

### Opção 1: Docker (Recomendado)

```bash
# Desenvolvimento
docker-compose --profile dev up --build

# Produção  
docker-compose --profile prod up --build
```

### Opção 2: Execução Local

```bash
# Pré-requisitos
- .NET 8.0 SDK

# Executar
cd Hackathon.API
dotnet run
```

### Executar Testes

```bash
# Todos os testes
dotnet test

# Testes específicos
dotnet test Hackathon.API.Tests
dotnet test Hackathon.Application.Tests
dotnet test Hackathon.Domain.Tests
dotnet test Hackathon.Infrastructure.Tests
```

### Acessar Documentação Interativa

```
http://localhost:5000/swagger
```

A documentação Swagger permite testar todos os endpoints diretamente no navegador, facilitando o entendimento e uso da API.

## 📈 Métricas de Sucesso

- **Tempo de resposta**: Processamento ágil de simulações
- **Disponibilidade**: Alta disponibilidade e confiabilidade
- **Cobertura de testes**: Cobertura abrangente de testes em todas as camadas
- **Documentação**: Documentação completa de todos os endpoints
- **Escalabilidade**: Suporte a alto volume de requisições simultâneas

## 🔍 Endpoints Principais

### Simulação de Crédito
```http
POST /simulacao
```
Realiza simulação completa com múltiplos métodos de amortização (SAC e PRICE)

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

## 🌟 Diferenciais da Solução Proposta

### Transparência Total
- Todos os cálculos são visíveis e compreensíveis
- Comparação lado a lado entre múltiplos métodos de amortização (SAC e PRICE)
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
