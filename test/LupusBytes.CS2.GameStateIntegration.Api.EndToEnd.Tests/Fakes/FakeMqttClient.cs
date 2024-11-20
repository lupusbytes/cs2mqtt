using LupusBytes.CS2.GameStateIntegration.Mqtt;

namespace LupusBytes.CS2.GameStateIntegration.Api.EndToEnd.Tests.Fakes;

internal sealed class FakeMqttClient : IMqttClient
{
    public Task PublishAsync(MqttMessage message, CancellationToken cancellationToken) => Task.CompletedTask;
}