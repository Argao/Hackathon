using FluentAssertions;
using Hackathon.API.Contracts.Requests;

namespace Hackathon.API.Tests.Contracts.Requests;

public class ListarSimulacoesRequestTests
{
    [Fact]
    public void ListarSimulacoesRequest_DeveTerPropriedadesCorretas()
    {
        // Arrange & Act
        var request = new ListarSimulacoesRequest(2, 20);

        // Assert
        request.Pagina.Should().Be(2);
        request.QtdRegistrosPagina.Should().Be(20);
    }

    [Fact]
    public void ListarSimulacoesRequest_ComValoresPadrao_DeveFuncionar()
    {
        // Arrange & Act
        var request = new ListarSimulacoesRequest(0, 0);

        // Assert
        request.Pagina.Should().Be(0);
        request.QtdRegistrosPagina.Should().Be(0);
    }

    [Fact]
    public void ListarSimulacoesRequest_ComValoresNegativos_DeveAceitar()
    {
        // Arrange & Act
        var request = new ListarSimulacoesRequest(-1, -10);

        // Assert
        request.Pagina.Should().Be(-1);
        request.QtdRegistrosPagina.Should().Be(-10);
    }

    [Fact]
    public void ListarSimulacoesRequest_ComValoresZero_DeveAceitar()
    {
        // Arrange & Act
        var request = new ListarSimulacoesRequest(0, 0);

        // Assert
        request.Pagina.Should().Be(0);
        request.QtdRegistrosPagina.Should().Be(0);
    }
}
