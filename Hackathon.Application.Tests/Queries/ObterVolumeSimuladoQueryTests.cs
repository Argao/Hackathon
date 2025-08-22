using Hackathon.Application.Queries;

namespace Hackathon.Application.Tests.Queries;

public class ObterVolumeSimuladoQueryTests
{
    [Fact]
    public void Constructor_ComDataValida_DeveCriarQuery()
    {
        // Arrange
        var dataReferencia = DateOnly.FromDateTime(DateTime.Today);

        // Act
        var query = new ObterVolumeSimuladoQuery(dataReferencia);

        // Assert
        query.Should().NotBeNull();
        query.DataReferencia.Should().Be(dataReferencia);
    }

    [Fact]
    public void Constructor_ComDataPassada_DeveCriarQuery()
    {
        // Arrange
        var dataReferencia = DateOnly.FromDateTime(DateTime.Today.AddDays(-1));

        // Act
        var query = new ObterVolumeSimuladoQuery(dataReferencia);

        // Assert
        query.Should().NotBeNull();
        query.DataReferencia.Should().Be(dataReferencia);
    }

    [Fact]
    public void Constructor_ComDataMuitoPassada_DeveCriarQuery()
    {
        // Arrange
        var dataReferencia = DateOnly.FromDateTime(DateTime.Today.AddYears(-10));

        // Act
        var query = new ObterVolumeSimuladoQuery(dataReferencia);

        // Assert
        query.Should().NotBeNull();
        query.DataReferencia.Should().Be(dataReferencia);
    }

    [Fact]
    public void Constructor_ComDataHoje_DeveCriarQuery()
    {
        // Arrange
        var dataReferencia = DateOnly.FromDateTime(DateTime.Today);

        // Act
        var query = new ObterVolumeSimuladoQuery(dataReferencia);

        // Assert
        query.Should().NotBeNull();
        query.DataReferencia.Should().Be(dataReferencia);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(7)]
    [InlineData(30)]
    [InlineData(365)]
    public void Constructor_ComDatasPassadasDiferentes_DeveCriarQuery(int diasPassados)
    {
        // Arrange
        var dataReferencia = DateOnly.FromDateTime(DateTime.Today.AddDays(-diasPassados));

        // Act
        var query = new ObterVolumeSimuladoQuery(dataReferencia);

        // Assert
        query.Should().NotBeNull();
        query.DataReferencia.Should().Be(dataReferencia);
    }

    [Fact]
    public void Constructor_ComDataLimiteHoje_DeveCriarQuery()
    {
        // Arrange
        var dataReferencia = DateOnly.FromDateTime(DateTime.Today);

        // Act
        var query = new ObterVolumeSimuladoQuery(dataReferencia);

        // Assert
        query.Should().NotBeNull();
        query.DataReferencia.Should().Be(dataReferencia);
    }

    [Fact]
    public void Constructor_ComDataLimiteOntem_DeveCriarQuery()
    {
        // Arrange
        var dataReferencia = DateOnly.FromDateTime(DateTime.Today.AddDays(-1));

        // Act
        var query = new ObterVolumeSimuladoQuery(dataReferencia);

        // Assert
        query.Should().NotBeNull();
        query.DataReferencia.Should().Be(dataReferencia);
    }

    [Fact]
    public void Constructor_ComDataEspecifica_DeveCriarQuery()
    {
        // Arrange
        var dataReferencia = new DateOnly(2024, 1, 1);

        // Act
        var query = new ObterVolumeSimuladoQuery(dataReferencia);

        // Assert
        query.Should().NotBeNull();
        query.DataReferencia.Should().Be(dataReferencia);
    }
}
