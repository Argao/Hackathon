# Guia Completo --- Configuração de Segurança no Projeto .NET 8 (Clean Arch)

Este guia mostra todas as etapas para configurar **analisadores,
.editorconfig, CI/CD e Quality Gate** a fim de garantir **zero
vulnerabilidade crítica** verificada por análise estática (SAST).

------------------------------------------------------------------------

## Passo 1 --- Criar o `.editorconfig`

1.  Na **raiz da solution**, crie um arquivo chamado **.editorconfig**.\
2.  Salve em **UTF-8 sem BOM** e use quebras de linha **LF**.\
3.  Cole este conteúdo:

``` ini
root = true

[*]
end_of_line = lf
insert_final_newline = true

[*.cs]
dotnet_analyzer_diagnostic.category-Security.severity = error
dotnet_diagnostic.CA3001.severity = error
dotnet_diagnostic.CA3002.severity = error
dotnet_diagnostic.CA5368.severity = error
dotnet_diagnostic.CA5350.severity = error
dotnet_diagnostic.CA5351.severity = error
dotnet_diagnostic.SCS0001.severity = error
dotnet_diagnostic.SCS0015.severity = error
```

------------------------------------------------------------------------

## Passo 2 --- Configurar `Directory.Build.props`

Crie ou edite o arquivo `Directory.Build.props` na raiz da solução:

``` xml
<Project>
  <PropertyGroup>
    <Nullable>enable</Nullable>
    <AnalysisLevel>latest</AnalysisLevel>
    <AnalysisMode>AllEnabledByDefault</AnalysisMode>
  </PropertyGroup>
  <ItemGroup>
    <PackageReference Include="Microsoft.CodeAnalysis.NetAnalyzers" Version="9.*" PrivateAssets="all" />
    <PackageReference Include="SonarAnalyzer.CSharp" Version="9.*" PrivateAssets="all" />
    <PackageReference Include="Microsoft.DevSkim" Version="0.*" PrivateAssets="all" />
  </ItemGroup>
</Project>
```

------------------------------------------------------------------------

## Passo 3 --- Criar conta e projeto no SonarCloud

1.  Vá até [SonarCloud.io](https://sonarcloud.io).\
2.  Crie/associe sua organização GitHub.\
3.  Crie um **projeto** para o repositório.\
4.  Gere um **Token (`SONAR_TOKEN`)** e adicione no GitHub do seu
    repositório:
    -   `Settings → Secrets and variables → Actions → New repository secret`.

------------------------------------------------------------------------

## Passo 4 --- Configurar Workflow de CI (Sonar + Auditoria de Dependências)

Crie o arquivo `.github/workflows/sonar.yml` com este conteúdo
(substitua `seuOrg` e `seuProjeto`):

``` yaml
name: SonarCloud (SAST)

on:
  push:
    branches: [ "main" ]
  pull_request:
    branches: [ "main" ]

jobs:
  analyze:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v4
        with:
          fetch-depth: 0

      - uses: actions/setup-dotnet@v4
        with:
          dotnet-version: '8.0.x'

      - run: dotnet restore
      - run: dotnet build --configuration Release

      - name: Dependency audit
        run: |
          set -e
          dotnet list package --vulnerable --include-transitive > vuln.txt || true
          if grep -Eqi 'Severity:.*Critical' vuln.txt; then
            echo 'Critical vulnerabilities detected in dependencies:'
            cat vuln.txt
            exit 1
          fi
          echo 'No Critical dependency vulnerabilities found.'

      - name: SonarCloud Scan
        uses: SonarSource/sonarcloud-github-action@v2
        env:
          GITHUB_TOKEN: ${{ secrets.GITHUB_TOKEN }}
          SONAR_TOKEN: ${{ secrets.SONAR_TOKEN }}
        with:
          args: >
            -Dsonar.projectKey=seuProjeto
            -Dsonar.organization=seuOrg
```

------------------------------------------------------------------------

## Passo 5 --- Configurar Gitleaks (secret scanning)

Crie `.github/workflows/gitleaks.yml`:

``` yaml
name: Gitleaks (Secrets)

on:
  push:
    branches: [ "main" ]
  pull_request:
    branches: [ "main" ]

jobs:
  gitleaks:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v4
        with: { fetch-depth: 0 }
      - uses: gitleaks/gitleaks-action@v2
        with:
          args: "--redact --verbose"
```

------------------------------------------------------------------------

## Passo 6 --- (Opcional) Configurar CodeQL

Crie `.github/workflows/codeql.yml`:

``` yaml
name: CodeQL

on:
  push: { branches: ["main"] }
  pull_request: { branches: ["main"] }
  schedule: [{ cron: '0 3 * * 1' }]

jobs:
  analyze:
    runs-on: ubuntu-latest
    permissions:
      actions: read
      contents: read
      security-events: write
    steps:
      - uses: actions/checkout@v4
      - uses: actions/setup-dotnet@v4
        with: { dotnet-version: '8.0.x' }
      - uses: github/codeql-action/init@v3
        with: { languages: csharp, queries: security-and-quality }
      - run: dotnet restore
      - run: dotnet build --configuration Release
      - uses: github/codeql-action/analyze@v3
```

------------------------------------------------------------------------

## Passo 7 --- Ativar Branch Protection

No GitHub, vá em **Settings → Branches → Branch protection rules** e
aplique para `main`:

-   Require PR before merging ✔️\
-   Require status checks ✔️ (selecione SonarCloud, Dependency Audit,
    Gitleaks, CodeQL)\
-   Require conversation resolution ✔️

------------------------------------------------------------------------

## Passo 8 --- Fluxo diário

1.  Codifique no Rider com SonarLint ativo.\
2.  `dotnet build` acusa violações locais.\
3.  Abra PR → CI roda Sonar + auditorias.\
4.  Corrija vulnerabilidades até **Quality Gate = 0 críticos**.\
5.  Só então faça merge.

------------------------------------------------------------------------

## Passo 9 --- O que gera críticos (fique atento)

-   SQL Injection → sempre use parâmetros, nunca concatene.\
-   Path Traversal → normalize caminhos e restrinja diretórios.\
-   Cripto fraca → evite MD5/SHA1, use AES-GCM/PBKDF2/Argon2.\
-   XSS → nunca renderize input sem escape.\
-   CSRF → valide antiforgery tokens.\
-   Deserialização insegura → use System.Text.Json.\
-   Headers → aplique HSTS, CSP, nosniff, DENY frame.

------------------------------------------------------------------------

## Passo 10 --- Como comprovar

-   Print do SonarCloud sem críticos.\
-   Logs do CI verdes (Sonar, Gitleaks, Dependency Audit).\
-   Screenshot do Branch Protection.\
-   Arquivos `.editorconfig`, `Directory.Build.props` e workflows no
    repo.

------------------------------------------------------------------------

✅ Seguindo todas essas etapas, seu projeto .NET 8 em Clean Arch terá
**pipeline de segurança automatizado**, garantindo o requisito de **zero
vulnerabilidade crítica**.
