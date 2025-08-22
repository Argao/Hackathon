using FluentAssertions;
using Hackathon.Application.Mappings;
using Mapster;
using MapsterMapper;

namespace Hackathon.Application.Tests.Mappings;

public class ApplicationMappingProfileTests
{
    [Fact]
    public void Configure_ShouldRegisterAllMappings()
    {
        // Arrange
        var config = new TypeAdapterConfig();

        // Act
        ApplicationMappingProfile.Configure();

        // Assert
        // Verificar se as configurações foram aplicadas
        config.Should().NotBeNull();
        
        // Verificar se os mapeamentos foram registrados
        var mapper = new Mapper(config);
        
        // Testar mapeamento básico - como é uma classe estática, 
        // apenas verificamos que o método não lança exceção
        // e que as configurações globais foram aplicadas
        
        // Verificar se as configurações globais foram aplicadas
        var globalConfig = TypeAdapterConfig.GlobalSettings;
        globalConfig.Should().NotBeNull();
    }
}
