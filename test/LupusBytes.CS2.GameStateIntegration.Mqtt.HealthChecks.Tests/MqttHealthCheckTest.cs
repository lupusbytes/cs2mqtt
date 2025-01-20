using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace LupusBytes.CS2.GameStateIntegration.Mqtt.HealthChecks.Tests;

public class MqttHealthCheckTest
{
    [Theory, AutoNSubstituteData]
    public async Task CheckHealthAsync_returns_Healthy(
        [Frozen] IMqttClient mqttClient,
        HealthCheckContext context,
        MqttHealthCheck sut)
    {
        // Arrange
        mqttClient.IsConnected.Returns(true);

        // Act
        var result = await sut.CheckHealthAsync(context, CancellationToken.None);

        // Assert
        result.Status.Should().Be(HealthStatus.Healthy);
    }

    [Theory, AutoNSubstituteData]
    public async Task CheckHealthAsync_returns_Failure(
        [Frozen] IMqttClient mqttClient,
        HealthCheckContext context,
        MqttHealthCheck sut)
    {
        // Arrange
        mqttClient.IsConnected.Returns(false);

        // Act
        var result = await sut.CheckHealthAsync(context, CancellationToken.None);

        // Assert
        result.Status.Should().Be(context.Registration.FailureStatus);
    }
}