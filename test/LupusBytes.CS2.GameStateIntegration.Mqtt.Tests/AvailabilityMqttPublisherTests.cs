using LupusBytes.CS2.GameStateIntegration.Contracts;
using LupusBytes.CS2.GameStateIntegration.Events;

namespace LupusBytes.CS2.GameStateIntegration.Mqtt.Tests;

public class AvailabilityMqttPublisherTests
{
    [Theory, AutoNSubstituteData]
    public async Task Publishes_system_availability_on_startup(
        [Frozen] IMqttClient mqttClient,
        AvailabilityMqttPublisher sut)
    {
        // Arrange
        using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));
        var tcs = new TaskCompletionSource<bool>();
        cts.Token.Register(() => tcs.TrySetCanceled(CancellationToken.None));
        mqttClient
            .When(x => x.PublishAsync(
                Arg.Is<MqttMessage>(m => m.Topic == MqttConstants.SystemAvailabilityTopic),
                Arg.Any<CancellationToken>()))
            .Do(_ => tcs.SetResult(true));

        // Act
        await sut.StartAsync(CancellationToken.None);

        // Assert
        await tcs.Task;
        await mqttClient.Received(1).PublishAsync(
            Arg.Is<MqttMessage>(x =>
                x.Topic == MqttConstants.SystemAvailabilityTopic &&
                x.Payload == "online" &&
                x.RetainFlag),
            Arg.Any<CancellationToken>());
    }

    [Theory, AutoNSubstituteData]
    public async Task Publishes_player_availability(
        [Frozen] IMqttClient mqttClient,
        SteamId64 steamId,
        Player player,
        AvailabilityMqttPublisher sut)
    {
        // Arrange
        using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));
        var tcs = new TaskCompletionSource<bool>();
        cts.Token.Register(() => tcs.TrySetCanceled(CancellationToken.None));
        mqttClient
            .When(x => x.PublishAsync(
                Arg.Is<MqttMessage>(m => m.Topic == $"{MqttConstants.BaseTopic}/{steamId}/player/status"),
                Arg.Any<CancellationToken>()))
            .Do(_ => tcs.SetResult(true));

        await sut.StartAsync(CancellationToken.None);

        // Act
        sut.OnNext(new PlayerEvent(steamId, player));

        // Assert
        await tcs.Task;
        await mqttClient.Received(1).PublishAsync(
            Arg.Is<MqttMessage>(x =>
                x.Topic == $"{MqttConstants.BaseTopic}/{steamId}/player/status" &&
                x.Payload == "online" &&
                x.RetainFlag),
            Arg.Any<CancellationToken>());
    }
}