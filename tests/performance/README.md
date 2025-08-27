# 🚀 Testes de Performance - Hackathon API

Este diretório contém scripts de teste de performance usando **k6** para a API de Simulação de Crédito.

## 📋 Pré-requisitos

### 1. Instalar k6
```bash
# Ubuntu/Debian
sudo gpg -k
sudo gpg --no-default-keyring --keyring /usr/share/keyrings/k6-archive-keyring.gpg --keyserver hkp://keyserver.ubuntu.com:80 --recv-keys C5AD17C747E3415A3642D57D77C6C491D6AC1D69
echo "deb [signed-by=/usr/share/keyrings/k6-archive-keyring.gpg] https://dl.k6.io/deb stable main" | sudo tee /etc/apt/sources.list.d/k6.list
sudo apt-get update
sudo apt-get install k6

# macOS
brew install k6

# Windows
choco install k6
```

### 2. Verificar instalação
```bash
k6 version
```

## 🎯 Tipos de Teste

### 1. **Baseline** (`baseline.js`)
- **Objetivo**: Estabelecer linha de base de performance
- **Configuração**: 1 usuário, 30s
- **Uso**: `./scripts/performance-test.sh baseline`

### 2. **Load** (`load.js`)
- **Objetivo**: Testar carga normal esperada
- **Configuração**: 50 usuários, 60s (configurável)
- **Uso**: `./scripts/performance-test.sh load [usuários] [duração]`

### 3. **Stress** (`stress.js`)
- **Objetivo**: Encontrar ponto de quebra
- **Configuração**: 10-100 usuários progressivos
- **Uso**: `./scripts/performance-test.sh stress`

### 4. **Spike** (`spike.js`)
- **Objetivo**: Testar picos de carga
- **Configuração**: 10 → 100 → 100 → 10 usuários
- **Uso**: `./scripts/performance-test.sh spike`

### 5. **Endurance** (`endurance.js`)
- **Objetivo**: Testar estabilidade por longo período
- **Configuração**: 20 usuários, 5 minutos (configurável)
- **Uso**: `./scripts/performance-test.sh endurance [duração]`

### 6. **Endpoint Específico** (`endpoint.js`)
- **Objetivo**: Testar endpoint específico
- **Configuração**: Configurável
- **Uso**: `./scripts/performance-test.sh endpoint [path] [usuários] [duração]`

## 📊 Produtos Disponíveis

Os testes usam dados reais dos produtos cadastrados no banco:

| Produto | Taxa Mensal | Prazo (meses) | Valor (R$) |
|---------|-------------|---------------|------------|
| **Produto 1** | 1.79% | 0-24 | 200,00 - 10.000,00 |
| **Produto 2** | 1.75% | 25-48 | 10.001,00 - 100.000,00 |
| **Produto 3** | 1.82% | 49-96 | 100.000,01 - 1.000.000,00 |
| **Produto 4** | 1.51% | 96+ | 1.000.000,01+ |

### Distribuição de Dados
- **Valores**: 70% médios, 20% baixos, 10% altos
- **Prazos**: 60% médios, 30% curtos, 10% longos
- **Data**: Sempre a data atual (YYYY-MM-DD)

## 🚀 Como Usar

### Comando Principal
```bash
./scripts/performance-test.sh [COMANDO] [OPÇÕES]
```

### Exemplos de Uso

#### 1. Verificar saúde da API
```bash
./scripts/performance-test.sh health
```

#### 2. Teste de baseline
```bash
./scripts/performance-test.sh baseline
```

#### 3. Teste de carga personalizado
```bash
./scripts/performance-test.sh load 25 120  # 25 usuários, 2 minutos
```

#### 4. Teste de stress progressivo
```bash
./scripts/performance-test.sh stress
```

#### 5. Teste de spike
```bash
./scripts/performance-test.sh spike
```

#### 6. Teste de endurance (10 minutos)
```bash
./scripts/performance-test.sh endurance 600
```

#### 7. Teste específico de endpoint
```bash
./scripts/performance-test.sh endpoint /simulacao 20 60
```

#### 8. Suite completa
```bash
./scripts/performance-test.sh full
```

#### 9. Analisar resultados
```bash
./scripts/performance-test.sh analyze
```

## 📊 Resultados

### Localização dos Resultados
- **Diretório**: `./test-results/`
- **Formato**: JSON (k6) + Markdown (relatório)
- **Timestamp**: Automático para cada execução

### Estrutura de Arquivos
```
test-results/
├── baseline_20240115_143022.json
├── load_50users_60s_20240115_143022.json
├── stress_10users_20240115_143022.json
├── stress_20users_20240115_143022.json
├── ...
├── report_20240115_143022.md
└── performance_test_20240115_143022.log
```

### Análise de Resultados

#### 1. **Métricas Principais**
- **RPS**: Requisições por segundo
- **Latência**: Tempo de resposta (min, médio, max, p95, p99)
- **Taxa de Erro**: Percentual de requisições com falha
- **Throughput**: Dados transferidos por segundo

#### 2. **Thresholds Configurados**
- **Baseline**: p95 < 1s, erro < 1%
- **Load**: p95 < 2s, erro < 5%
- **Stress**: p95 < 5s, erro < 10%
- **Spike**: p95 < 3s, erro < 10%
- **Endurance**: p95 < 2s, erro < 5%

## 🔧 Configurações

### Variáveis de Ambiente
```bash
# URL da API
BASE_URL="http://localhost:8080"

# Diretório de resultados
RESULTS_DIR="./test-results"

# Configurações padrão
USERS=50
DURATION=60
RAMP_UP=10
```

### Personalização
Edite o arquivo `scripts/performance-test.sh` para modificar:
- URL base da API
- Diretório de resultados
- Configurações padrão
- Cores do output

## 📈 Interpretação dos Resultados

### ✅ **Bom Desempenho**
- RPS > 50
- P95 < 1s
- Taxa de erro < 1%
- Sem degradação progressiva

### ⚠️ **Atenção Necessária**
- RPS < 20
- P95 > 2s
- Taxa de erro > 5%
- Degradação com aumento de carga

### ❌ **Problemas Críticos**
- RPS < 10
- P95 > 5s
- Taxa de erro > 10%
- Falhas em cascata

## 🛠️ Troubleshooting

### Problemas Comuns

#### 1. **k6 não encontrado**
```bash
# Verificar instalação
which k6
k6 version
```

#### 2. **API não responde**
```bash
# Verificar se a API está rodando
curl http://localhost:8080/health
```

#### 3. **Erro de permissão**
```bash
# Tornar script executável
chmod +x scripts/performance-test.sh
```

#### 4. **Resultados não gerados**
```bash
# Verificar diretório de resultados
ls -la test-results/
```

### Logs Detalhados
- **Log principal**: `test-results/performance_test_[timestamp].log`
- **Logs k6**: Console durante execução
- **Relatórios**: `test-results/report_[timestamp].md`

## 🔄 Comparação de Resultados

### Antes vs Depois das Otimizações
```bash
# Executar teste antes das mudanças
./scripts/performance-test.sh baseline

# Fazer mudanças na aplicação

# Executar teste depois das mudanças
./scripts/performance-test.sh baseline

# Comparar resultados
diff test-results/baseline_before.json test-results/baseline_after.json
```

## 📝 Próximos Passos

1. **Executar suite completa** para baseline
2. **Implementar otimizações** identificadas
3. **Re-executar testes** para comparar
4. **Analisar gargalos** específicos
5. **Otimizar endpoints** problemáticos
6. **Configurar monitoramento** contínuo

---

**Status**: ✅ Pronto para uso  
**Última Atualização**: Janeiro 2024  
**Responsável**: Equipe de Performance
