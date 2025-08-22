using FluentAssertions;
using Hackathon.Domain.ValueObjects;
using Xunit;

namespace Hackathon.Domain.Tests.ValueObjects;

public class ResultTests
{
    [Fact]
    public void Success_DeveRetornarResultadoComSucesso()
    {
        // Act
        var result = Result.Success();

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.IsFailure.Should().BeFalse();
        result.Error.Should().BeEmpty();
    }

    [Fact]
    public void Failure_DeveRetornarResultadoComFalha()
    {
        // Arrange
        var mensagemErro = "Erro de teste";

        // Act
        var result = Result.Failure(mensagemErro);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(mensagemErro);
    }

    [Fact]
    public void Failure_ComMensagemNula_DeveUsarStringVazia()
    {
        // Act
        var result = Result.Failure(null!);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.IsFailure.Should().BeTrue();
        result.Error.Should().BeEmpty();
    }

    [Fact]
    public void ConversaoImplicita_ParaBool_DeveFuncionar()
    {
        // Arrange
        var sucesso = Result.Success();
        var falha = Result.Failure("Erro");

        // Act
        bool sucessoBool = sucesso;
        bool falhaBool = falha;

        // Assert
        sucessoBool.Should().BeTrue();
        falhaBool.Should().BeFalse();
    }

    [Fact]
    public void RecordStruct_DeveTerIgualdadePorValor()
    {
        // Arrange
        var result1 = Result.Success();
        var result2 = Result.Success();
        var result3 = Result.Failure("Erro");

        // Act & Assert
        result1.Should().Be(result2);
        result1.Should().NotBe(result3);
    }
}

public class ResultTTests
{
    [Fact]
    public void Success_DeveRetornarResultadoComSucessoEValor()
    {
        // Arrange
        var valor = "teste";

        // Act
        var result = Result<string>.Success(valor);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.IsFailure.Should().BeFalse();
        result.Value.Should().Be(valor);
        result.Error.Should().BeEmpty();
    }

    [Fact]
    public void Failure_DeveRetornarResultadoComFalha()
    {
        // Arrange
        var mensagemErro = "Erro de teste";

        // Act
        var result = Result<string>.Failure(mensagemErro);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(mensagemErro);
        result.Value.Should().Be(default(string));
    }

    [Fact]
    public void Failure_ComMensagemNula_DeveUsarStringVazia()
    {
        // Act
        var result = Result<string>.Failure(null!);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.IsFailure.Should().BeTrue();
        result.Error.Should().BeEmpty();
    }

    [Fact]
    public void ConversaoImplicita_ParaBool_DeveFuncionar()
    {
        // Arrange
        var sucesso = Result<string>.Success("teste");
        var falha = Result<string>.Failure("Erro");

        // Act
        bool sucessoBool = sucesso;
        bool falhaBool = falha;

        // Assert
        sucessoBool.Should().BeTrue();
        falhaBool.Should().BeFalse();
    }

    [Fact]
    public void ConversaoImplicita_ParaT_DeveFuncionar()
    {
        // Arrange
        var valor = "teste";
        var result = Result<string>.Success(valor);

        // Act
        string valorConvertido = result;

        // Assert
        valorConvertido.Should().Be(valor);
    }

    [Fact]
    public void ConversaoImplicita_ParaT_ComFalha_DeveRetornarDefault()
    {
        // Arrange
        var result = Result<string>.Failure("Erro");

        // Act
        string valorConvertido = result;

        // Assert
        valorConvertido.Should().Be(default(string));
    }

    [Fact]
    public void RecordStruct_DeveTerIgualdadePorValor()
    {
        // Arrange
        var result1 = Result<string>.Success("teste");
        var result2 = Result<string>.Success("teste");
        var result3 = Result<string>.Success("outro");
        var result4 = Result<string>.Failure("Erro");

        // Act & Assert
        result1.Should().Be(result2);
        result1.Should().NotBe(result3);
        result1.Should().NotBe(result4);
    }

    [Fact]
    public void ComTiposPrimitivos_DeveFuncionarCorretamente()
    {
        // Arrange & Act
        var intResult = Result<int>.Success(42);
        var decimalResult = Result<decimal>.Success(100.50m);
        var boolResult = Result<bool>.Success(true);

        // Assert
        intResult.Value.Should().Be(42);
        decimalResult.Value.Should().Be(100.50m);
        boolResult.Value.Should().BeTrue();
    }

    [Fact]
    public void ComObjetosComplexos_DeveFuncionarCorretamente()
    {
        // Arrange
        var objeto = new { Nome = "Teste", Id = 1 };

        // Act
        var result = Result<object>.Success(objeto);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be(objeto);
    }
}
