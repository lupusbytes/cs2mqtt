using Microsoft.Extensions.DependencyInjection;

namespace LupusBytes.CS2.GameStateIntegration.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddGameStateService(this IServiceCollection services)
        => services.AddSingleton<IGameStateService, GameStateService>();
}