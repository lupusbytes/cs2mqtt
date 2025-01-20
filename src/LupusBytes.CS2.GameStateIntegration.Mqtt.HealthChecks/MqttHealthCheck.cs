using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace LupusBytes.CS2.GameStateIntegration.Mqtt.HealthChecks;

public class MqttHealthCheck(IMqttClient mqttClient) : IHealthCheck
{
    public Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
        => Task.FromResult(mqttClient.IsConnected
            ? HealthCheckResult.Healthy("MQTT connection is established.")
            : new HealthCheckResult(context.Registration.FailureStatus, "MQTT connection is not established."));
}