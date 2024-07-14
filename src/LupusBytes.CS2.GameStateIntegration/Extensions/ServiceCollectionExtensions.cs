using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace LupusBytes.CS2.GameStateIntegration.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddGameStateService(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var options = new GameStateOptions();
        configuration.GetSection(GameStateOptions.Section).Bind(options);
        services.AddSingleton(options);

        services.AddSingleton<IGameStateService, GameStateService>();

        return services;
    }
}