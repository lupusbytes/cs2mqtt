using Microsoft.AspNetCore.Diagnostics.HealthChecks;

namespace LupusBytes.CS2.GameStateIntegration.Api.Extensions;

internal static class EndpointRouteBuilderExtensions
{
    internal static void MapHealthCheckEndpoints(this IEndpointRouteBuilder app)
    {
        // All health checks must pass for app to be considered ready to accept traffic after starting
        app.MapHealthChecks("/health");

        // Only health checks tagged with the "live" tag must pass for app to be considered alive
        app.MapHealthChecks("/alive", new HealthCheckOptions
        {
            Predicate = static r => r.Tags.Contains("live"),
        });
    }
}