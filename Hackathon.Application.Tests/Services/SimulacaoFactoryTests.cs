using Hackathon.Application.Services;
using Hackathon.Domain.Entities;
using Hackathon.Domain.ValueObjects;
using Hackathon.Domain.Exceptions;

namespace Hackathon.Application.Tests.Services;

public class SimulacaoFactoryTests
{
    private readonly SimulacaoFactory _factory;

    public SimulacaoFactoryTests()
    {
        _factory = new SimulacaoFactory();
    }

    [Fact]
    public void CriarSimulacao_ComParametrosValidos_DeveCriarSimulacaoCorretamente()
    {
        // Arrange
        var codigoProduto = 1;
        var descricaoProduto = "Empréstimo Pessoal";
        var taxaJuros = TaxaJuros.Create(0.05m).Value;
        var valorDesejado = ValorMonetario.Create(10000m).Value;
        var prazoMeses = PrazoMeses.Create(12).Value;

        // Act
        var simulacao = _factory.CriarSimulacao(codigoProduto, descricaoProduto, taxaJuros, valorDesejado, prazoMeses);

        // Assert
        Assert.NotNull(simulacao);
        Assert.Equal(codigoProduto, simulacao.CodigoProduto);
        Assert.Equal(descricaoProduto, simulacao.DescricaoProduto);
        Assert.Equal(taxaJuros, simulacao.TaxaJuros);
        Assert.Equal(valorDesejado, simulacao.ValorDesejado);
        Assert.Equal(prazoMeses, simulacao.PrazoMeses);
        Assert.Equal(DateOnly.FromDateTime(DateTime.Today), simulacao.DataReferencia);
    }

    [Fact]
    public void CriarSimulacao_ComDiferentesProdutos_DeveCriarSimulacoesDistintas()
    {
        // Arrange
        var taxaJuros = TaxaJuros.Create(0.05m).Value;
        var valorDesejado = ValorMonetario.Create(10000m).Value;
        var prazoMeses = PrazoMeses.Create(12).Value;

        // Act
        var simulacao1 = _factory.CriarSimulacao(1, "Empréstimo Pessoal", taxaJuros, valorDesejado, prazoMeses);
        var simulacao2 = _factory.CriarSimulacao(2, "Financiamento Imobiliário", taxaJuros, valorDesejado, prazoMeses);

        // Assert
        Assert.NotEqual(simulacao1.CodigoProduto, simulacao2.CodigoProduto);
        Assert.NotEqual(simulacao1.DescricaoProduto, simulacao2.DescricaoProduto);
        Assert.Equal(taxaJuros, simulacao1.TaxaJuros);
        Assert.Equal(taxaJuros, simulacao2.TaxaJuros);
        Assert.Equal(valorDesejado, simulacao1.ValorDesejado);
        Assert.Equal(valorDesejado, simulacao2.ValorDesejado);
    }

    [Fact]
    public void CriarSimulacao_ComDiferentesValores_DeveCriarSimulacoesComValoresCorretos()
    {
        // Arrange
        var codigoProduto = 1;
        var descricaoProduto = "Empréstimo Pessoal";
        var taxaJuros = TaxaJuros.Create(0.05m).Value;
        var prazoMeses = PrazoMeses.Create(12).Value;

        // Act
        var simulacao1 = _factory.CriarSimulacao(codigoProduto, descricaoProduto, taxaJuros, ValorMonetario.Create(5000m).Value, prazoMeses);
        var simulacao2 = _factory.CriarSimulacao(codigoProduto, descricaoProduto, taxaJuros, ValorMonetario.Create(15000m).Value, prazoMeses);

        // Assert
        Assert.Equal(ValorMonetario.Create(5000m).Value, simulacao1.ValorDesejado);
        Assert.Equal(ValorMonetario.Create(15000m).Value, simulacao2.ValorDesejado);
    }

    [Fact]
    public void CriarSimulacao_ComDiferentesPrazos_DeveCriarSimulacoesComPrazosCorretos()
    {
        // Arrange
        var codigoProduto = 1;
        var descricaoProduto = "Empréstimo Pessoal";
        var taxaJuros = TaxaJuros.Create(0.05m).Value;
        var valorDesejado = ValorMonetario.Create(10000m).Value;

        // Act
        var simulacao1 = _factory.CriarSimulacao(codigoProduto, descricaoProduto, taxaJuros, valorDesejado, PrazoMeses.Create(6).Value);
        var simulacao2 = _factory.CriarSimulacao(codigoProduto, descricaoProduto, taxaJuros, valorDesejado, PrazoMeses.Create(24).Value);

        // Assert
        Assert.Equal(PrazoMeses.Create(6).Value, simulacao1.PrazoMeses);
        Assert.Equal(PrazoMeses.Create(24).Value, simulacao2.PrazoMeses);
    }

    [Fact]
    public void CriarSimulacao_ComDiferentesTaxas_DeveCriarSimulacoesComTaxasCorretas()
    {
        // Arrange
        var codigoProduto = 1;
        var descricaoProduto = "Empréstimo Pessoal";
        var valorDesejado = ValorMonetario.Create(10000m).Value;
        var prazoMeses = PrazoMeses.Create(12).Value;

        // Act
        var simulacao1 = _factory.CriarSimulacao(codigoProduto, descricaoProduto, TaxaJuros.Create(0.03m).Value, valorDesejado, prazoMeses);
        var simulacao2 = _factory.CriarSimulacao(codigoProduto, descricaoProduto, TaxaJuros.Create(0.07m).Value, valorDesejado, prazoMeses);

        // Assert
        Assert.Equal(TaxaJuros.Create(0.03m).Value, simulacao1.TaxaJuros);
        Assert.Equal(TaxaJuros.Create(0.07m).Value, simulacao2.TaxaJuros);
    }

    [Fact]
    public void CriarSimulacao_ComDescricaoVazia_DeveLancarExcecao()
    {
        // Arrange
        var codigoProduto = 1;
        var descricaoProduto = "";
        var taxaJuros = TaxaJuros.Create(0.05m).Value;
        var valorDesejado = ValorMonetario.Create(10000m).Value;
        var prazoMeses = PrazoMeses.Create(12).Value;

        // Act & Assert
        var exception = Assert.Throws<BusinessRuleException>(() => 
            _factory.CriarSimulacao(codigoProduto, descricaoProduto, taxaJuros, valorDesejado, prazoMeses));
        
        Assert.Equal("Descrição do produto é obrigatória", exception.Message);
    }

    [Fact]
    public void CriarSimulacao_ComDescricaoNull_DeveLancarExcecao()
    {
        // Arrange
        var codigoProduto = 1;
        string descricaoProduto = null;
        var taxaJuros = TaxaJuros.Create(0.05m).Value;
        var valorDesejado = ValorMonetario.Create(10000m).Value;
        var prazoMeses = PrazoMeses.Create(12).Value;

        // Act & Assert
        var exception = Assert.Throws<BusinessRuleException>(() => 
            _factory.CriarSimulacao(codigoProduto, descricaoProduto, taxaJuros, valorDesejado, prazoMeses));
        
        Assert.Equal("Descrição do produto é obrigatória", exception.Message);
    }

    [Fact]
    public void CriarSimulacao_ComCodigoProdutoZero_DeveCriarSimulacaoComCodigoZero()
    {
        // Arrange
        var codigoProduto = 0;
        var descricaoProduto = "Produto Teste";
        var taxaJuros = TaxaJuros.Create(0.05m).Value;
        var valorDesejado = ValorMonetario.Create(10000m).Value;
        var prazoMeses = PrazoMeses.Create(12).Value;

        // Act
        var simulacao = _factory.CriarSimulacao(codigoProduto, descricaoProduto, taxaJuros, valorDesejado, prazoMeses);

        // Assert
        Assert.Equal(0, simulacao.CodigoProduto);
    }

    [Fact]
    public void CriarSimulacao_ComCodigoProdutoNegativo_DeveCriarSimulacaoComCodigoNegativo()
    {
        // Arrange
        var codigoProduto = -1;
        var descricaoProduto = "Produto Teste";
        var taxaJuros = TaxaJuros.Create(0.05m).Value;
        var valorDesejado = ValorMonetario.Create(10000m).Value;
        var prazoMeses = PrazoMeses.Create(12).Value;

        // Act
        var simulacao = _factory.CriarSimulacao(codigoProduto, descricaoProduto, taxaJuros, valorDesejado, prazoMeses);

        // Assert
        Assert.Equal(-1, simulacao.CodigoProduto);
    }
}
