using FluentAssertions;
using Hackathon.Domain.ValueObjects;
using Xunit;

namespace Hackathon.Domain.Tests.ValueObjects;

public class RegrasNegocioTests
{
    [Fact]
    public void Valores_DeveTerLimitesConsistentes()
    {
        RegrasNegocio.Valores.VALOR_MINIMO_EMPRESTIMO.Should().Be(0.01m);
        RegrasNegocio.Valores.VALOR_MAXIMO_EMPRESTIMO.Should().Be(999_999_999.99m);
        RegrasNegocio.Valores.VALOR_MINIMO_MONETARIO.Should().Be(0.00m);
        RegrasNegocio.Valores.VALOR_MAXIMO_MONETARIO.Should().Be(999_999_999_999.99m);
    }

    [Fact]
    public void Prazos_DeveTerLimitesConsistentes()
    {
        RegrasNegocio.Prazos.PRAZO_MINIMO_MESES.Should().Be(1);
        RegrasNegocio.Prazos.PRAZO_MAXIMO_MESES.Should().Be(600);
        RegrasNegocio.Prazos.PRAZO_MAXIMO_API.Should().Be(360);
    }

    [Fact]
    public void Taxas_DeveTerLimitesConsistentes()
    {
        RegrasNegocio.Taxas.TAXA_MINIMA.Should().Be(0.000001m);
        RegrasNegocio.Taxas.TAXA_MAXIMA.Should().Be(0.50m);
    }
}
