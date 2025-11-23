using Hackathon.API.Mappings;
using Mapster;

namespace Hackathon.API.Tests.Mappings;

public static class MapsterTestConfig
{
    private static readonly object Sync = new();
    private static bool _configured;

    public static void EnsureConfigured()
    {
        if (_configured)
        {
            return;
        }

        lock (Sync)
        {
            if (_configured)
            {
                return;
            }

            ApiMappingProfile.Configure(TypeAdapterConfig.GlobalSettings);
            TypeAdapterConfig.GlobalSettings.Compile();
            _configured = true;
        }
    }
}
