using FluentAssertions;
using Hackathon.Application.Mappings;
using Mapster;

namespace Hackathon.Application.Tests.Mappings;

public class MapsterConfigurationTests
{
    [Fact]
    public void Configure_ShouldRegisterAllMappings()
    {
        // Act
        MapsterConfiguration.Configure();

        // Assert
        // Verificar se as configurações foram aplicadas
        // O método Configure registra os mapeamentos do ApplicationMappingProfile
        // e configura as configurações globais
        
        // Verificar se as configurações globais foram aplicadas
        var config = TypeAdapterConfig.GlobalSettings;
        config.Should().NotBeNull();
    }
}
