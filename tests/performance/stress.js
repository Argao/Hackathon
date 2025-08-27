import http from 'k6/http';
import { check, sleep } from 'k6';
import { Rate } from 'k6/metrics';

// Configurações
const BASE_URL = __ENV.BASE_URL || 'http://localhost:8080';
const TEST_TYPE = __ENV.TEST_TYPE || 'stress';
const USERS = parseInt(__ENV.USERS) || 100;
const DURATION = parseInt(__ENV.DURATION) || 120;
const RAMP_UP = parseInt(__ENV.RAMP_UP) || 30;

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

// Função para gerar valor realista baseado no produto (mais extremo para stress)
function gerarValorRealista(produto) {
    const min = produto.minValor;
    const max = produto.maxValor || 999999999.99;
    
    // Para stress, gerar valores mais extremos
    const random = Math.random();
    let valor;
    
    if (random < 0.4) {
        // Valores baixos (40% dos casos)
        valor = min + (max - min) * Math.random() * 0.2;
    } else if (random < 0.8) {
        // Valores altos (40% dos casos)
        valor = min + (max - min) * (0.8 + Math.random() * 0.2);
    } else {
        // Valores extremos (20% dos casos)
        valor = min + (max - min) * (0.95 + Math.random() * 0.05);
    }
    
    return Math.round(valor);
}

// Função para gerar prazo realista baseado no produto (mais extremo para stress)
function gerarPrazoRealista(produto) {
    const min = produto.minMeses;
    const max = produto.maxMeses || 600;
    
    // Para stress, gerar prazos mais extremos
    const random = Math.random();
    let prazo;
    
    if (random < 0.4) {
        // Prazos curtos (40% dos casos)
        prazo = min + Math.round((max - min) * Math.random() * 0.2);
    } else if (random < 0.8) {
        // Prazos longos (40% dos casos)
        prazo = min + Math.round((max - min) * (0.8 + Math.random() * 0.2));
    } else {
        // Prazos extremos (20% dos casos)
        prazo = min + Math.round((max - min) * (0.95 + Math.random() * 0.05));
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
        { duration: `${RAMP_UP}s`, target: USERS }, // Ramp-up gradual
        { duration: `${DURATION}s`, target: USERS }, // Carga máxima
        { duration: '20s', target: 0 }, // Ramp-down
    ],
    thresholds: {
        http_req_duration: ['p(95)<5000'], // 95% das requisições devem ser < 5s
        http_req_failed: ['rate<0.10'], // Taxa de erro < 10%
        errors: ['rate<0.10'],
    },
};

// Função de setup
export function setup() {
    console.log(`💥 Iniciando teste de stress`);
    console.log(`📊 Configurações:`);
    console.log(`   • URL Base: ${BASE_URL}`);
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
    
    // Para teste de stress, focar nos endpoints mais pesados
    const endpoints = [
        { method: 'POST', path: '/simulacao', name: 'Criar Simulação', weight: 50 },
        { method: 'GET', path: `/simulacao/volume-por-dia?dataReferencia=${dataAtual}`, name: 'Volume por Dia', weight: 30 },
        { method: 'GET', path: `/telemetria/por-dia?dataReferencia=${dataAtual}`, name: 'Telemetria por Dia', weight: 20 },
    ];
    
    // Selecionar endpoint baseado no peso
    const totalWeight = endpoints.reduce((sum, ep) => sum + ep.weight, 0);
    let random = Math.random() * totalWeight;
    let selectedEndpoint = endpoints[0];
    
    for (const endpoint of endpoints) {
        random -= endpoint.weight;
        if (random <= 0) {
            selectedEndpoint = endpoint;
            break;
        }
    }
    
    let response;
    
    // Executar requisição baseada no método
    switch (selectedEndpoint.method) {
        case 'GET':
            response = http.get(`${baseUrl}${selectedEndpoint.path}`);
            break;
        case 'POST':
            // Dados de simulação variados para stress
            const produto = PRODUTOS_REAIS[Math.floor(Math.random() * PRODUTOS_REAIS.length)];
            const payload = {
                valorDesejado: gerarValorRealista(produto),
                prazo: gerarPrazoRealista(produto)
            };
            response = http.post(`${baseUrl}${selectedEndpoint.path}`, JSON.stringify(payload), {
                headers: { 'Content-Type': 'application/json' },
            });
            break;
        default:
            response = http.get(`${baseUrl}${selectedEndpoint.path}`);
    }
    
    // Verificações mais permissivas para stress
    const checks = check(response, {
        [`${selectedEndpoint.name} - status is 200`]: (r) => r.status === 200,
        [`${selectedEndpoint.name} - response time < 3000ms`]: (r) => r.timings.duration < 3000,
        [`${selectedEndpoint.name} - response has content`]: (r) => r.body.length > 0,
    });
    
    // Registrar erro se a verificação falhou
    errorRate.add(!checks);
    
    // Sleep mínimo para stress (máximo de carga)
    sleep(0.05);
}

// Função de teardown
export function teardown(data) {
    console.log(`✅ Teste de stress concluído`);
}
