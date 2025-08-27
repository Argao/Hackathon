import http from 'k6/http';
import { check, sleep } from 'k6';
import { Rate } from 'k6/metrics';

// Configurações
const BASE_URL = __ENV.BASE_URL || 'http://localhost:8080';
const TEST_TYPE = __ENV.TEST_TYPE || 'endpoint';
const ENDPOINT = __ENV.ENDPOINT || '/simulacao';
const USERS = parseInt(__ENV.USERS) || 20;
const DURATION = parseInt(__ENV.DURATION) || 60;
const RAMP_UP = parseInt(__ENV.RAMP_UP) || 10;

// Métricas customizadas
const errorRate = new Rate('errors');

// Produtos reais baseados nos dados do banco
const PRODUTOS_REAIS = [
    {
        codigo: 1,
        descricao: "Produto 1",
        taxaMensal: 0.0179, // 1.79% ao mês
        minMeses: 0,
        maxMeses: 24,
        minValor: 200.00,
        maxValor: 10000.00
    },
    {
        codigo: 2,
        descricao: "Produto 2",
        taxaMensal: 0.0175, // 1.75% ao mês
        minMeses: 25,
        maxMeses: 48,
        minValor: 10001.00,
        maxValor: 100000.00
    },
    {
        codigo: 3,
        descricao: "Produto 3",
        taxaMensal: 0.0182, // 1.82% ao mês
        minMeses: 49,
        maxMeses: 96,
        minValor: 100000.01,
        maxValor: 1000000.00
    },
    {
        codigo: 4,
        descricao: "Produto 4",
        taxaMensal: 0.0151, // 1.51% ao mês
        minMeses: 96,
        maxMeses: null, // Sem limite máximo
        minValor: 1000000.01,
        maxValor: null // Sem limite máximo
    }
];

// Função para gerar valor realista baseado no produto
function gerarValorRealista(produto) {
    const min = produto.minValor;
    const max = produto.maxValor || 999999999.99; // Limite máximo do sistema
    
    // Distribuição mais realista: 70% valores médios, 20% baixos, 10% altos
    const random = Math.random();
    let valor;
    
    if (random < 0.7) {
        // Valores médios (70% dos casos)
        valor = min + (max - min) * (0.3 + Math.random() * 0.4);
    } else if (random < 0.9) {
        // Valores baixos (20% dos casos)
        valor = min + (max - min) * Math.random() * 0.3;
    } else {
        // Valores altos (10% dos casos)
        valor = min + (max - min) * (0.7 + Math.random() * 0.3);
    }
    
    return Math.round(valor);
}

// Função para gerar prazo realista baseado no produto
function gerarPrazoRealista(produto) {
    const min = produto.minMeses;
    const max = produto.maxMeses || 600; // Limite máximo do sistema
    
    // Distribuição mais realista: 60% prazos médios, 30% curtos, 10% longos
    const random = Math.random();
    let prazo;
    
    if (random < 0.6) {
        // Prazos médios (60% dos casos)
        prazo = min + Math.round((max - min) * (0.3 + Math.random() * 0.4));
    } else if (random < 0.9) {
        // Prazos curtos (30% dos casos)
        prazo = min + Math.round((max - min) * Math.random() * 0.3);
    } else {
        // Prazos longos (10% dos casos)
        prazo = min + Math.round((max - min) * (0.7 + Math.random() * 0.3));
    }
    
    return Math.max(min, Math.min(max, prazo));
}

// Função para obter data atual no formato correto
function obterDataAtual() {
    const hoje = new Date();
    return hoje.toISOString().split('T')[0]; // YYYY-MM-DD
}

// Configuração do teste
export const options = {
    stages: [
        { duration: `${RAMP_UP}s`, target: USERS }, // Ramp-up
        { duration: `${DURATION}s`, target: USERS }, // Carga constante
        { duration: '10s', target: 0 }, // Ramp-down
    ],
    thresholds: {
        http_req_duration: ['p(95)<2000'], // 95% das requisições devem ser < 2s
        http_req_failed: ['rate<0.05'], // Taxa de erro < 5%
        errors: ['rate<0.05'],
    },
};

// Função de setup
export function setup() {
    console.log(`🎯 Iniciando teste de endpoint específico`);
    console.log(`📊 Configurações:`);
    console.log(`   • URL Base: ${BASE_URL}`);
    console.log(`   • Endpoint: ${ENDPOINT}`);
    console.log(`   • Usuários: ${USERS}`);
    console.log(`   • Duração: ${DURATION}s`);
    console.log(`   • Ramp-up: ${RAMP_UP}s`);
    console.log(`   • Data de referência: ${obterDataAtual()}`);
    console.log(`   • Produtos disponíveis: ${PRODUTOS_REAIS.length}`);
    
    // Verificar se a API está respondendo
    const healthCheck = http.get(`${BASE_URL}/health`);
    check(healthCheck, {
        'health check passed': (r) => r.status === 200,
    });
    
    return { baseUrl: BASE_URL };
}

// Função principal do teste
export default function(data) {
    const baseUrl = data.baseUrl;
    const dataAtual = obterDataAtual();
    
    let response;
    let endpointName = ENDPOINT;
    
    // Determinar método e preparar dados baseado no endpoint
    if (ENDPOINT === '/simulacao') {
        // POST para criar simulação
        const produto = PRODUTOS_REAIS[Math.floor(Math.random() * PRODUTOS_REAIS.length)];
        const payload = {
            valorDesejado: gerarValorRealista(produto),
            prazo: gerarPrazoRealista(produto)
        };
        response = http.post(`${baseUrl}${ENDPOINT}`, JSON.stringify(payload), {
            headers: { 'Content-Type': 'application/json' },
        });
        endpointName = 'Criar Simulação';
    } else if (ENDPOINT === '/simulacao/volume-por-dia') {
        // GET com parâmetro de data
        response = http.get(`${baseUrl}${ENDPOINT}?dataReferencia=${dataAtual}`);
        endpointName = 'Volume por Dia';
    } else if (ENDPOINT === '/telemetria/por-dia') {
        // GET com parâmetro de data
        response = http.get(`${baseUrl}${ENDPOINT}?dataReferencia=${dataAtual}`);
        endpointName = 'Telemetria por Dia';
    } else {
        // GET padrão para outros endpoints
        response = http.get(`${baseUrl}${ENDPOINT}`);
    }
    
    // Verificações
    const checks = check(response, {
        [`${endpointName} - status is 200`]: (r) => r.status === 200,
        [`${endpointName} - response time < 1500ms`]: (r) => r.timings.duration < 1500,
        [`${endpointName} - response has content`]: (r) => r.body.length > 0,
    });
    
    // Registrar erro se a verificação falhou
    errorRate.add(!checks);
    
    // Sleep entre requisições
    sleep(0.2);
}

// Função de teardown
export function teardown(data) {
    console.log(`✅ Teste de endpoint específico concluído`);
}
