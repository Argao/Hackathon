#!/bin/bash

# ===========================
# Script de Testes com Cobertura - Hackathon API
# ===========================

set -e

# Cores para output
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
RED='\033[0;31m'
BLUE='\033[0;34m'
NC='\033[0m' # No Color

print_success() {
    echo -e "${GREEN}✓ $1${NC}"
}

print_warning() {
    echo -e "${YELLOW}⚠ $1${NC}"
}

print_error() {
    echo -e "${RED}✗ $1${NC}"
}

print_info() {
    echo -e "${BLUE}ℹ $1${NC}"
}

print_header() {
    echo -e "${YELLOW}================================${NC}"
    echo -e "${YELLOW}  $1${NC}"
    echo -e "${YELLOW}================================${NC}"
}

# Verificar se dotCover está disponível
check_dotcover() {
    print_warning "Verificando dotCover..."
    
    if command -v dotcover &> /dev/null; then
        print_success "dotCover encontrado"
        return 0
    else
        print_warning "dotCover não encontrado, usando cobertura nativa do .NET"
        return 1
    fi
}

# Executar testes com cobertura usando dotCover
run_tests_with_dotcover() {
    print_header "Executando Testes com dotCover"
    
    # Configuração do dotCover
    local config_file="dotcover.config"
    
    # Criar arquivo de configuração do dotCover
    cat > "$config_file" << EOF
<?xml version="1.0" encoding="utf-8"?>
<CoverageParams>
  <TargetExecutable>dotnet</TargetExecutable>
  <TargetArguments>test --no-build --verbosity quiet</TargetArguments>
  <TargetWorkingDir>.</TargetWorkingDir>
  <InheritConsole>true</InheritConsole>
  <AnalyzeTargetArguments>false</AnalyzeTargetArguments>
  <MergeByHash>true</MergeByHash>
  <UseNewSnapshot>true</UseNewSnapshot>
  <AllowSymbolServerAccess>true</AllowSymbolServerAccess>
  <Filters>
    <IncludeFilters>
      <FilterEntry>
        <ModuleMask>*Hackathon*</ModuleMask>
        <ClassMask>*</ClassMask>
        <FunctionMask>*</FunctionMask>
      </FilterEntry>
    </IncludeFilters>
    <ExcludeFilters>
      <FilterEntry>
        <ModuleMask>*Tests*</ModuleMask>
        <ClassMask>*</ClassMask>
        <FunctionMask>*</FunctionMask>
      </FilterEntry>
      <FilterEntry>
        <ModuleMask>*Test*</ModuleMask>
        <ClassMask>*</ClassMask>
        <FunctionMask>*</FunctionMask>
      </FilterEntry>
    </ExcludeFilters>
  </Filters>
  <AttributeFilters>
    <AttributeFilterEntry>
      <AttributeMask>*GeneratedCode*</AttributeMask>
    </AttributeFilterEntry>
    <AttributeFilterEntry>
      <AttributeMask>*CompilerGenerated*</AttributeMask>
    </AttributeFilterEntry>
  </AttributeFilters>
</CoverageParams>
EOF

    print_info "Configuração do dotCover criada"
    
    # Executar testes com dotCover
    print_warning "Executando testes com cobertura..."
    
    if dotcover cover "$config_file" --Output=coverage-report.html --ReportType=HTML; then
        print_success "Testes executados com sucesso!"
        print_success "Relatório de cobertura gerado: coverage-report.html"
        
        # Tentar abrir o relatório no navegador
        if command -v xdg-open &> /dev/null; then
            xdg-open coverage-report.html
        elif command -v open &> /dev/null; then
            open coverage-report.html
        else
            print_info "Abra o arquivo coverage-report.html no seu navegador"
        fi
    else
        print_error "Erro ao executar testes com dotCover"
        exit 1
    fi
    
    # Limpar arquivo de configuração
    rm -f "$config_file"
}

# Executar testes com cobertura nativa do .NET
run_tests_with_native_coverage() {
    print_header "Executando Testes com Cobertura Nativa"
    
    print_warning "Executando testes com cobertura..."
    
    # Instalar coverlet se necessário
    if ! dotnet tool list -g | grep -q coverlet; then
        print_info "Instalando coverlet..."
        dotnet tool install -g coverlet.console
    fi
    
    # Executar testes com cobertura
    if dotnet test --collect:"XPlat Code Coverage" --results-directory ./coverage; then
        print_success "Testes executados com sucesso!"
        print_success "Relatórios de cobertura salvos em ./coverage/"
        
        # Listar arquivos gerados
        if [ -d "./coverage" ]; then
            print_info "Arquivos de cobertura gerados:"
            ls -la ./coverage/
        fi
    else
        print_error "Erro ao executar testes"
        exit 1
    fi
}

# Executar testes simples sem cobertura
run_simple_tests() {
    print_header "Executando Testes Simples"
    
    print_warning "Executando todos os testes..."
    
    if dotnet test --verbosity normal; then
        print_success "Todos os testes passaram!"
    else
        print_error "Alguns testes falharam"
        exit 1
    fi
}

# Função principal
main() {
    print_header "Testes e Cobertura - Hackathon API"
    
    # Verificar argumentos
    case "${1:-all}" in
        "dotcover")
            if check_dotcover; then
                run_tests_with_dotcover
            else
                print_error "dotCover não disponível"
                exit 1
            fi
            ;;
        "coverage")
            run_tests_with_native_coverage
            ;;
        "simple")
            run_simple_tests
            ;;
        "all")
            print_info "Executando todos os tipos de teste..."
            run_simple_tests
            echo ""
            if check_dotcover; then
                run_tests_with_dotcover
            else
                run_tests_with_native_coverage
            fi
            ;;
        *)
            print_error "Uso: $0 [dotcover|coverage|simple|all]"
            print_info "  dotcover  - Executar com dotCover (se disponível)"
            print_info "  coverage  - Executar com cobertura nativa do .NET"
            print_info "  simple    - Executar testes simples"
            print_info "  all       - Executar todos os tipos (padrão)"
            exit 1
            ;;
    esac
    
    print_header "Testes Concluídos!"
    print_success "✅ Qualidade do código verificada"
    print_success "📊 Cobertura de testes analisada"
    print_success "🚀 Pronto para produção!"
}

# Executar função principal
main "$@"
