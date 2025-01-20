using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace LupusBytes.CS2.GameStateIntegration.Mqtt.HealthChecks.Extensions;

public static class HealthCheckBuilderExtensions
{
    public static IHealthChecksBuilder AddMqtt(
        this IHealthChecksBuilder builder,
        string name = "mqtt",
        HealthStatus? failureStatus = null,
        IEnumerable<string>? tags = null,
        TimeSpan? timeout = null)
        => builder.Add(
            new HealthCheckRegistration(
                name,
                sp => new MqttHealthCheck(sp.GetRequiredService<IMqttClient>()),
                failureStatus,
                tags,
                timeout));
}