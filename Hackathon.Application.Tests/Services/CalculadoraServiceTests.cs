using Hackathon.Application.Services;
using Hackathon.Domain.Entities;
using Hackathon.Domain.Enums;
using Hackathon.Domain.Interfaces.Services;
using Hackathon.Domain.ValueObjects;

namespace Hackathon.Application.Tests.Services;

public class CalculadoraServiceTests
{
    private readonly Mock<ICalculadoraAmortizacao> _mockCalculadora1;
    private readonly Mock<ICalculadoraAmortizacao> _mockCalculadora2;
    private readonly CalculadoraService _service;

    public CalculadoraServiceTests()
    {
        _mockCalculadora1 = new Mock<ICalculadoraAmortizacao>();
        _mockCalculadora2 = new Mock<ICalculadoraAmortizacao>();
        
        var calculadoras = new List<ICalculadoraAmortizacao>
        {
            _mockCalculadora1.Object,
            _mockCalculadora2.Object
        };

        _service = new CalculadoraService(calculadoras);
    }

    [Fact]
    public void ExecutarCalculos_ComParametrosValidos_DeveExecutarTodasCalculadoras()
    {
        // Arrange
        var valorEmprestimo = ValorMonetario.Create(10000m).Value;
        var taxaJuros = TaxaJuros.Create(0.05m).Value;
        var prazoMeses = PrazoMeses.Create(12).Value;

        var resultado1 = new ResultadoSimulacao { Tipo = SistemaAmortizacao.SAC, ValorTotal = ValorMonetario.Create(1000m).Value };
        var resultado2 = new ResultadoSimulacao { Tipo = SistemaAmortizacao.PRICE, ValorTotal = ValorMonetario.Create(1050m).Value };

        _mockCalculadora1.Setup(x => x.Calcular(valorEmprestimo, taxaJuros, prazoMeses))
            .Returns(resultado1);
        _mockCalculadora2.Setup(x => x.Calcular(valorEmprestimo, taxaJuros, prazoMeses))
            .Returns(resultado2);

        // Act
        var resultados = _service.ExecutarCalculos(valorEmprestimo, taxaJuros, prazoMeses);

        // Assert
        Assert.NotNull(resultados);
        Assert.Equal(2, resultados.Count);
        Assert.Contains(resultado1, resultados);
        Assert.Contains(resultado2, resultados);

        _mockCalculadora1.Verify(x => x.Calcular(valorEmprestimo, taxaJuros, prazoMeses), Times.Once);
        _mockCalculadora2.Verify(x => x.Calcular(valorEmprestimo, taxaJuros, prazoMeses), Times.Once);
    }

    [Fact]
    public void ExecutarCalculos_ComListaVaziaDeCalculadoras_DeveRetornarListaVazia()
    {
        // Arrange
        var calculadoras = new List<ICalculadoraAmortizacao>();
        var service = new CalculadoraService(calculadoras);

        var valorEmprestimo = ValorMonetario.Create(10000m).Value;
        var taxaJuros = TaxaJuros.Create(0.05m).Value;
        var prazoMeses = PrazoMeses.Create(12).Value;

        // Act
        var resultados = service.ExecutarCalculos(valorEmprestimo, taxaJuros, prazoMeses);

        // Assert
        Assert.NotNull(resultados);
        Assert.Empty(resultados);
    }

    [Fact]
    public void ExecutarCalculos_ComCalculadoraQueRetornaNull_DeveIncluirNullNaLista()
    {
        // Arrange
        var mockCalculadora = new Mock<ICalculadoraAmortizacao>();
        var calculadoras = new List<ICalculadoraAmortizacao> { mockCalculadora.Object };
        var service = new CalculadoraService(calculadoras);

        var valorEmprestimo = ValorMonetario.Create(10000m).Value;
        var taxaJuros = TaxaJuros.Create(0.05m).Value;
        var prazoMeses = PrazoMeses.Create(12).Value;

        mockCalculadora.Setup(x => x.Calcular(valorEmprestimo, taxaJuros, prazoMeses))
            .Returns((ResultadoSimulacao)null);

        // Act
        var resultados = service.ExecutarCalculos(valorEmprestimo, taxaJuros, prazoMeses);

        // Assert
        Assert.NotNull(resultados);
        Assert.Single(resultados);
        Assert.Null(resultados[0]);
    }

    [Fact]
    public void ExecutarCalculos_ComCalculadoraQueLancaExcecao_DevePropagarExcecao()
    {
        // Arrange
        var mockCalculadora = new Mock<ICalculadoraAmortizacao>();
        var calculadoras = new List<ICalculadoraAmortizacao> { mockCalculadora.Object };
        var service = new CalculadoraService(calculadoras);

        var valorEmprestimo = ValorMonetario.Create(10000m).Value;
        var taxaJuros = TaxaJuros.Create(0.05m).Value;
        var prazoMeses = PrazoMeses.Create(12).Value;

        var excecao = new InvalidOperationException("Erro no cálculo");
        mockCalculadora.Setup(x => x.Calcular(valorEmprestimo, taxaJuros, prazoMeses))
            .Throws(excecao);

        // Act & Assert
        var exception = Assert.Throws<InvalidOperationException>(() => 
            service.ExecutarCalculos(valorEmprestimo, taxaJuros, prazoMeses));
        
        Assert.Equal("Erro no cálculo", exception.Message);
    }
}
