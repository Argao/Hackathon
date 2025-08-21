using Mapster;

namespace Hackathon.Application.Mappings;

/// <summary>
/// Configuração centralizada do Mapster
/// </summary>
public static class MapsterConfiguration
{
    /// <summary>
    /// Configura todos os mapeamentos do Mapster
    /// </summary>
    public static void Configure()
    {
        // Configurações globais do Mapster
        TypeAdapterConfig.GlobalSettings.Scan(typeof(MapsterConfiguration).Assembly);
        
        // Aplicar configurações específicas
        ApplicationMappingProfile.Configure();
        
        // Configurações adicionais
        ConfigureGlobalSettings();
    }

    private static void ConfigureGlobalSettings()
    {
        TypeAdapterConfig.GlobalSettings.Default
            .PreserveReference(true)
            .IgnoreNullValues(true);
    }
}
