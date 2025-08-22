using FluentAssertions;
using Hackathon.Domain.Entities;
using Hackathon.Domain.ValueObjects;
using Xunit;

namespace Hackathon.Domain.Tests.Entities;

public class ProdutoTests
{
    [Fact]
    public void Construtor_DeveCriarProdutoComValoresPadrao()
    {
        // Act
        var produto = new Produto();

        // Assert
        produto.Codigo.Should().Be(0);
        produto.Descricao.Should().Be(string.Empty);
        produto.TaxaMensal.Should().Be(default(TaxaJuros));
        produto.MinMeses.Should().Be(0);
        produto.MaxMeses.Should().BeNull();
        produto.MinValor.Should().Be(default(ValorMonetario));
        produto.MaxValor.Should().BeNull();
    }

    [Fact]
    public void Propriedades_DevePermitirDefinirEObterValores()
    {
        // Arrange
        var produto = new Produto();
        var codigo = 123;
        var descricao = "Empréstimo Pessoal";
        var taxaMensal = TaxaJuros.Create(0.015m).Value;
        var minMeses = (short)12;
        var maxMeses = (short?)60;
        var minValor = ValorMonetario.Create(1000m).Value;
        var maxValor = ValorMonetario.Create(50000m).Value;

        // Act
        produto.Codigo = codigo;
        produto.Descricao = descricao;
        produto.TaxaMensal = taxaMensal;
        produto.MinMeses = minMeses;
        produto.MaxMeses = maxMeses;
        produto.MinValor = minValor;
        produto.MaxValor = maxValor;

        // Assert
        produto.Codigo.Should().Be(codigo);
        produto.Descricao.Should().Be(descricao);
        produto.TaxaMensal.Should().Be(taxaMensal);
        produto.MinMeses.Should().Be(minMeses);
        produto.MaxMeses.Should().Be(maxMeses);
        produto.MinValor.Should().Be(minValor);
        produto.MaxValor.Should().Be(maxValor);
    }

    [Fact]
    public void AtendeRequisitos_ComValorEPrazoDentroDosLimites_DeveRetornarTrue()
    {
        // Arrange
        var produto = new Produto
        {
            MinValor = ValorMonetario.Create(1000m).Value,
            MaxValor = ValorMonetario.Create(50000m).Value,
            MinMeses = 12,
            MaxMeses = 60
        };
        var valor = ValorMonetario.Create(10000m).Value;
        var prazo = 24;

        // Act
        var resultado = produto.AtendeRequisitos(valor, prazo);

        // Assert
        resultado.Should().BeTrue();
    }

    [Fact]
    public void AtendeRequisitos_ComValorAbaixoDoMinimo_DeveRetornarFalse()
    {
        // Arrange
        var produto = new Produto
        {
            MinValor = ValorMonetario.Create(1000m).Value,
            MaxValor = ValorMonetario.Create(50000m).Value,
            MinMeses = 12,
            MaxMeses = 60
        };
        var valor = ValorMonetario.Create(500m).Value;
        var prazo = 24;

        // Act
        var resultado = produto.AtendeRequisitos(valor, prazo);

        // Assert
        resultado.Should().BeFalse();
    }

    [Fact]
    public void AtendeRequisitos_ComValorAcimaDoMaximo_DeveRetornarFalse()
    {
        // Arrange
        var produto = new Produto
        {
            MinValor = ValorMonetario.Create(1000m).Value,
            MaxValor = ValorMonetario.Create(50000m).Value,
            MinMeses = 12,
            MaxMeses = 60
        };
        var valor = ValorMonetario.Create(60000m).Value;
        var prazo = 24;

        // Act
        var resultado = produto.AtendeRequisitos(valor, prazo);

        // Assert
        resultado.Should().BeFalse();
    }

    [Fact]
    public void AtendeRequisitos_ComPrazoAbaixoDoMinimo_DeveRetornarFalse()
    {
        // Arrange
        var produto = new Produto
        {
            MinValor = ValorMonetario.Create(1000m).Value,
            MaxValor = ValorMonetario.Create(50000m).Value,
            MinMeses = 12,
            MaxMeses = 60
        };
        var valor = ValorMonetario.Create(10000m).Value;
        var prazo = 6;

        // Act
        var resultado = produto.AtendeRequisitos(valor, prazo);

        // Assert
        resultado.Should().BeFalse();
    }

    [Fact]
    public void AtendeRequisitos_ComPrazoAcimaDoMaximo_DeveRetornarFalse()
    {
        // Arrange
        var produto = new Produto
        {
            MinValor = ValorMonetario.Create(1000m).Value,
            MaxValor = ValorMonetario.Create(50000m).Value,
            MinMeses = 12,
            MaxMeses = 60
        };
        var valor = ValorMonetario.Create(10000m).Value;
        var prazo = 72;

        // Act
        var resultado = produto.AtendeRequisitos(valor, prazo);

        // Assert
        resultado.Should().BeFalse();
    }

    [Fact]
    public void AtendeRequisitos_ComMaxValorNulo_DeveAceitarQualquerValorAcimaDoMinimo()
    {
        // Arrange
        var produto = new Produto
        {
            MinValor = ValorMonetario.Create(1000m).Value,
            MaxValor = null,
            MinMeses = 12,
            MaxMeses = 60
        };
        var valor = ValorMonetario.Create(100000m).Value;
        var prazo = 24;

        // Act
        var resultado = produto.AtendeRequisitos(valor, prazo);

        // Assert
        resultado.Should().BeTrue();
    }

    [Fact]
    public void AtendeRequisitos_ComMaxMesesNulo_DeveAceitarQualquerPrazoAcimaDoMinimo()
    {
        // Arrange
        var produto = new Produto
        {
            MinValor = ValorMonetario.Create(1000m).Value,
            MaxValor = ValorMonetario.Create(50000m).Value,
            MinMeses = 12,
            MaxMeses = null
        };
        var valor = ValorMonetario.Create(10000m).Value;
        var prazo = 120;

        // Act
        var resultado = produto.AtendeRequisitos(valor, prazo);

        // Assert
        resultado.Should().BeTrue();
    }

    [Fact]
    public void AtendeRequisitos_ComValorExatamenteNoLimite_DeveRetornarTrue()
    {
        // Arrange
        var produto = new Produto
        {
            MinValor = ValorMonetario.Create(1000m).Value,
            MaxValor = ValorMonetario.Create(50000m).Value,
            MinMeses = 12,
            MaxMeses = 60
        };
        var valorMinimo = ValorMonetario.Create(1000m).Value;
        var valorMaximo = ValorMonetario.Create(50000m).Value;
        var prazoMinimo = 12;
        var prazoMaximo = 60;

        // Act & Assert
        produto.AtendeRequisitos(valorMinimo, prazoMinimo).Should().BeTrue();
        produto.AtendeRequisitos(valorMaximo, prazoMaximo).Should().BeTrue();
    }

    [Fact]
    public void AtendeRequisitos_ComValorExatamenteNoLimite_DeveRetornarFalse()
    {
        // Arrange
        var produto = new Produto
        {
            MinValor = ValorMonetario.Create(1000m).Value,
            MaxValor = ValorMonetario.Create(50000m).Value,
            MinMeses = 12,
            MaxMeses = 60
        };
        var valorAbaixoMinimo = ValorMonetario.Create(999.99m).Value;
        var valorAcimaMaximo = ValorMonetario.Create(50000.01m).Value;
        var prazoAbaixoMinimo = 11;
        var prazoAcimaMaximo = 61;

        // Act & Assert
        produto.AtendeRequisitos(valorAbaixoMinimo, 12).Should().BeFalse();
        produto.AtendeRequisitos(ValorMonetario.Create(1000m).Value, prazoAbaixoMinimo).Should().BeFalse();
        produto.AtendeRequisitos(valorAcimaMaximo, 60).Should().BeFalse();
        produto.AtendeRequisitos(ValorMonetario.Create(50000m).Value, prazoAcimaMaximo).Should().BeFalse();
    }

    [Fact]
    public void Produto_DeveSerClasseSealed()
    {
        // Act & Assert
        typeof(Produto).IsSealed.Should().BeTrue();
    }

    [Fact]
    public void Produto_DeveSerClassePublica()
    {
        // Act & Assert
        typeof(Produto).IsPublic.Should().BeTrue();
    }

    [Fact]
    public void Produto_DeveTerPropriedadesPublicas()
    {
        // Arrange
        var tipo = typeof(Produto);

        // Act & Assert
        tipo.GetProperty("Codigo")!.Should().NotBeNull();
        tipo.GetProperty("Descricao")!.Should().NotBeNull();
        tipo.GetProperty("TaxaMensal")!.Should().NotBeNull();
        tipo.GetProperty("MinMeses")!.Should().NotBeNull();
        tipo.GetProperty("MaxMeses")!.Should().NotBeNull();
        tipo.GetProperty("MinValor")!.Should().NotBeNull();
        tipo.GetProperty("MaxValor")!.Should().NotBeNull();
    }

    [Fact]
    public void AtendeRequisitos_DeveSerMetodoPublico()
    {
        // Arrange
        var tipo = typeof(Produto);

        // Act & Assert
        tipo.GetMethod("AtendeRequisitos")!.IsPublic.Should().BeTrue();
    }
}
