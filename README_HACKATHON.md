# 🚀 API Simulador de Crédito - Hackathon CAIXA

Bem-vindo ao futuro da simulação de crédito! Esta API foi desenvolvida como uma solução robusta, escalável e moderna para o desafio de Back-end do Hackathon CAIXA, permitindo que qualquer pessoa ou sistema descubra as melhores condições de empréstimo de forma rápida e segura.

## ✨ Inovações e Diferenciais

Este não é apenas mais um simulador. A arquitetura e as tecnologias foram escolhidas para entregar uma solução de alta performance e pronta para o futuro:

*   **Arquitetura Limpa (Clean Architecture):** O projeto foi estruturado seguindo os princípios da Arquitetura Limpa e do Domain-Driven Design (DDD). Isso garante um código desacoplado, testável, de fácil manutenção e evolução, separando claramente as regras de negócio da infraestrutura e da interface.
*   **Comunicação Assíncrona com Azure Event Hub:** Inovamos ao integrar a API com o Azure Event Hub. Cada simulação gera um evento que é publicado em tempo real. Isso simula uma arquitetura moderna e orientada a eventos, permitindo que outras áreas (como CRM, BI ou Análise de Risco) consumam esses dados de forma independente e sem impactar a performance da API principal.
*   **Observabilidade e Telemetria:** A API já nasce com um endpoint dedicado para telemetria (`/simulacoes/telemetria`), fornecendo métricas essenciais como volume de requisições e tempos de resposta. Isso demonstra uma preocupação com a monitoração e a performance em um ambiente produtivo.
*   **Containerização Completa com Docker:** A solução é 100% containerizada. Com um único comando, é possível subir a API e o banco de dados SQL Server, garantindo um ambiente de desenvolvimento e produção consistente e de fácil configuração.
*   **Cobertura de Testes:** O projeto conta com uma suíte de testes unitários e de integração, garantindo a qualidade, a confiabilidade e a corretude das regras de negócio e dos endpoints.

## 🛠️ Como Executar o Projeto

Você pode rodar a aplicação de duas formas: localmente na sua máquina ou via Docker (recomendado).

### 🐳 Executando com Docker (Recomendado)

Este é o método mais simples. Você só precisa ter o Docker e o Docker Compose instalados.

1.  **Clone o repositório (ou descompacte o arquivo .zip).**
2.  **Abra um terminal na raiz do projeto e execute o script de inicialização:**

    *   No **Linux ou macOS**:
        ```bash
        sh scripts/docker.sh
        ```
    *   No **Windows (PowerShell)**:
        ```powershell
        .\\scripts\\docker.ps1
        ```
    *   No **Windows (CMD)**:
        ```cmd
        .\\scripts\\docker.bat
        ```
3.  **Pronto!** O script irá:
    *   Construir a imagem da API.
    *   Subir os containers da API e do banco de dados SQL Server.
    *   Executar as migrações e popular o banco de dados com os produtos iniciais.

A API estará disponível em `http://localhost:8080`.

### 💻 Executando Localmente

Para rodar o projeto localmente, você precisará do **.NET 8 SDK** e de uma instância do **SQL Server** acessível.

1.  **Configure o Banco de Dados:**
    *   Crie um banco de dados no seu SQL Server.
    *   Atualize a string de conexão `DefaultConnection` no arquivo `Hackathon.API/appsettings.Development.json` com os dados do seu banco.

2.  **Restaure as dependências e execute as migrações:**
    *   Abra um terminal na raiz do projeto.
    *   Execute o script de setup para sua plataforma:
        *   No **Linux ou macOS**: `sh scripts/setup.sh`
        *   No **Windows (PowerShell)**: `.\\scripts\\setup.ps1`
        *   No **Windows (CMD)**: `.\\scripts\\setup.bat`

3.  **Inicie a Aplicação:**
    *   Navegue até o diretório da API: `cd Hackathon.API`
    *   Execute o comando: `dotnet run`

A API estará disponível em `http://localhost:5000` (ou outra porta configurada no `launchSettings.json`).

## 📄 Endpoints da API

A coleção do Postman com exemplos de requisições está na pasta `data` do projeto.

*   `POST /simulacoes`: Realiza uma nova simulação de crédito.
*   `GET /simulacoes`: Retorna todas as simulações realizadas.
*   `GET /simulacoes/relatorio`: Retorna os valores simulados agrupados por produto e dia.
*   `GET /simulacoes/telemetria`: Retorna dados de telemetria da API.

---
Este projeto foi construído com paixão e foco na entrega de uma solução de alta qualidade, demonstrando conhecimento em práticas modernas de desenvolvimento de software para atender aos desafios do mercado financeiro.

