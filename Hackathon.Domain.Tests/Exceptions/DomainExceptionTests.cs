using FluentAssertions;
using Hackathon.Domain.Exceptions;
using Xunit;

namespace Hackathon.Domain.Tests.Exceptions;

public class DomainExceptionTests
{
    [Fact]
    public void DomainException_DeveSerClasseAbstrata()
    {
        // Act & Assert
        typeof(DomainException).IsAbstract.Should().BeTrue();
    }

    [Fact]
    public void DomainException_DeveHerdarDeException()
    {
        // Act & Assert
        typeof(DomainException).Should().BeDerivedFrom<Exception>();
    }
}

public class BusinessRuleExceptionTests
{
    [Fact]
    public void Construtor_ComMensagem_DeveCriarExcecao()
    {
        // Arrange
        var mensagem = "Regra de negócio violada";

        // Act
        var exception = new BusinessRuleException(mensagem);

        // Assert
        exception.Message.Should().Be(mensagem);
        exception.InnerException.Should().BeNull();
    }

    [Fact]
    public void Construtor_ComMensagemEInnerException_DeveCriarExcecao()
    {
        // Arrange
        var mensagem = "Regra de negócio violada";
        var innerException = new InvalidOperationException("Erro interno");

        // Act
        var exception = new BusinessRuleException(mensagem, innerException);

        // Assert
        exception.Message.Should().Be(mensagem);
        exception.InnerException.Should().Be(innerException);
    }

    [Fact]
    public void BusinessRuleException_DeveHerdarDeDomainException()
    {
        // Act & Assert
        typeof(BusinessRuleException).Should().BeDerivedFrom<DomainException>();
    }
}

public class ValidationExceptionTests
{
    [Fact]
    public void Construtor_ComMensagem_DeveCriarExcecao()
    {
        // Arrange
        var mensagem = "Dados inválidos";

        // Act
        var exception = new ValidationException(mensagem);

        // Assert
        exception.Message.Should().Be(mensagem);
        exception.InnerException.Should().BeNull();
    }

    [Fact]
    public void Construtor_ComMensagem_DeveCriarExcecaoComErrors()
    {
        // Arrange
        var mensagem = "Dados inválidos";

        // Act
        var exception = new ValidationException(mensagem);

        // Assert
        exception.Message.Should().Be(mensagem);
        exception.Errors.Should().ContainSingle();
        exception.Errors.First().Should().Be(mensagem);
    }

    [Fact]
    public void Construtor_ComListaDeErros_DeveCriarExcecao()
    {
        // Arrange
        var erros = new[] { "Erro 1", "Erro 2", "Erro 3" };

        // Act
        var exception = new ValidationException(erros);

        // Assert
        exception.Message.Should().Be("Erro 1; Erro 2; Erro 3");
        exception.Errors.Should().BeEquivalentTo(erros);
    }

    [Fact]
    public void ValidationException_DeveHerdarDeDomainException()
    {
        // Act & Assert
        typeof(ValidationException).Should().BeDerivedFrom<DomainException>();
    }
}

public class NotFoundExceptionTests
{
    [Fact]
    public void Construtor_ComResourceName_DeveCriarExcecao()
    {
        // Arrange
        var resourceName = "Produto";

        // Act
        var exception = new NotFoundException(resourceName);

        // Assert
        exception.Message.Should().Be("Recurso 'Produto' não encontrado");
        exception.ResourceName.Should().Be(resourceName);
        exception.ResourceId.Should().BeNull();
    }

    [Fact]
    public void Construtor_ComResourceNameEResourceId_DeveCriarExcecao()
    {
        // Arrange
        var resourceName = "Produto";
        var resourceId = 123;

        // Act
        var exception = new NotFoundException(resourceName, resourceId);

        // Assert
        exception.Message.Should().Be("Recurso 'Produto' não encontrado com ID: 123");
        exception.ResourceName.Should().Be(resourceName);
        exception.ResourceId.Should().Be(resourceId);
    }

    [Fact]
    public void Construtor_ComMensagemEInnerException_DeveCriarExcecao()
    {
        // Arrange
        var mensagem = "Recurso não encontrado";
        var innerException = new FileNotFoundException("Arquivo não encontrado");

        // Act
        var exception = new NotFoundException(mensagem, innerException);

        // Assert
        exception.Message.Should().Be(mensagem);
        exception.InnerException.Should().Be(innerException);
    }

    [Fact]
    public void NotFoundException_DeveHerdarDeDomainException()
    {
        // Act & Assert
        typeof(NotFoundException).Should().BeDerivedFrom<DomainException>();
    }
}

public class ProdutoNotFoundExceptionTests
{
    [Fact]
    public void Construtor_ComCodigoProduto_DeveCriarExcecaoComMensagemApropriada()
    {
        // Arrange
        var codigoProduto = "123";

        // Act
        var exception = new ProdutoNotFoundException(codigoProduto);

        // Assert
        exception.Message.Should().Contain(codigoProduto);
        exception.Message.Should().Contain("não encontrado");
        exception.CodigoProduto.Should().Be(codigoProduto);
    }

    [Fact]
    public void ProdutoNotFoundException_DeveHerdarDeDomainException()
    {
        // Act & Assert
        typeof(ProdutoNotFoundException).Should().BeDerivedFrom<DomainException>();
    }
}

public class SimulacaoExceptionTests
{
    [Fact]
    public void Construtor_ComMensagem_DeveCriarExcecao()
    {
        // Arrange
        var mensagem = "Erro na simulação";

        // Act
        var exception = new SimulacaoException(mensagem);

        // Assert
        exception.Message.Should().Be(mensagem);
        exception.InnerException.Should().BeNull();
    }

    [Fact]
    public void Construtor_ComMensagemEInnerException_DeveCriarExcecao()
    {
        // Arrange
        var mensagem = "Erro na simulação";
        var innerException = new Exception("Erro interno");

        // Act
        var exception = new SimulacaoException(mensagem, innerException);

        // Assert
        exception.Message.Should().Be(mensagem);
        exception.InnerException.Should().Be(innerException);
    }

    [Fact]
    public void SimulacaoException_DeveHerdarDeDomainException()
    {
        // Act & Assert
        typeof(SimulacaoException).Should().BeDerivedFrom<DomainException>();
    }
}

public class InfrastructureExceptionTests
{
    [Fact]
    public void Construtor_ComMensagem_DeveCriarExcecao()
    {
        // Arrange
        var mensagem = "Erro de infraestrutura";

        // Act
        var exception = new InfrastructureException(mensagem);

        // Assert
        exception.Message.Should().Be(mensagem);
        exception.InnerException.Should().BeNull();
    }

    [Fact]
    public void Construtor_ComMensagemEInnerException_DeveCriarExcecao()
    {
        // Arrange
        var mensagem = "Erro de infraestrutura";
        var innerException = new Exception("Erro interno");

        // Act
        var exception = new InfrastructureException(mensagem, innerException);

        // Assert
        exception.Message.Should().Be(mensagem);
        exception.InnerException.Should().Be(innerException);
    }

    [Fact]
    public void InfrastructureException_DeveHerdarDeDomainException()
    {
        // Act & Assert
        typeof(InfrastructureException).Should().BeDerivedFrom<DomainException>();
    }
}
