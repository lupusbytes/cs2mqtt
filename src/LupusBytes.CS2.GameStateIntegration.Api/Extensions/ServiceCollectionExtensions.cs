using LupusBytes.CS2.GameStateIntegration.Mqtt.HealthChecks.Extensions;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace LupusBytes.CS2.GameStateIntegration.Api.Extensions;

internal static class ServiceCollectionExtensions
{
    internal static IServiceCollection ConfigureHealthChecks(this IServiceCollection services)
    {
        services
            .AddHealthChecks()
            .AddCheck("self", () => HealthCheckResult.Healthy(), ["live"])
            .AddMqtt();

        return services;
    }
}