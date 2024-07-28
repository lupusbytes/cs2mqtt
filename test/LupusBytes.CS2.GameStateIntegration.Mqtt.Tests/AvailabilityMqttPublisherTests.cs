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

    [Theory, AutoNSubstituteData]
    public async Task Publishes_player_state_availability(
        [Frozen] IMqttClient mqttClient,
        SteamId64 steamId,
        PlayerState playerState,
        AvailabilityMqttPublisher sut)
    {
        // Arrange
        using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));
        var tcs = new TaskCompletionSource<bool>();
        cts.Token.Register(() => tcs.TrySetCanceled(CancellationToken.None));
        mqttClient
            .When(x => x.PublishAsync(
                Arg.Is<MqttMessage>(m => m.Topic == $"{MqttConstants.BaseTopic}/{steamId}/player-state/status"),
                Arg.Any<CancellationToken>()))
            .Do(_ => tcs.SetResult(true));

        await sut.StartAsync(CancellationToken.None);

        // Act
        sut.OnNext(new PlayerStateEvent(steamId, playerState));

        // Assert
        await tcs.Task;
        await mqttClient.Received(1).PublishAsync(
            Arg.Is<MqttMessage>(x =>
                x.Topic == $"{MqttConstants.BaseTopic}/{steamId}/player-state/status" &&
                x.Payload == "online" &&
                x.RetainFlag),
            Arg.Any<CancellationToken>());
    }

    [Theory, AutoNSubstituteData]
    public async Task Publishes_map_availability(
        [Frozen] IMqttClient mqttClient,
        SteamId64 steamId,
        Map map,
        AvailabilityMqttPublisher sut)
    {
        // Arrange
        using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));
        var tcs = new TaskCompletionSource<bool>();
        cts.Token.Register(() => tcs.TrySetCanceled(CancellationToken.None));
        mqttClient
            .When(x => x.PublishAsync(
                Arg.Is<MqttMessage>(m => m.Topic == $"{MqttConstants.BaseTopic}/{steamId}/map/status"),
                Arg.Any<CancellationToken>()))
            .Do(_ => tcs.SetResult(true));

        await sut.StartAsync(CancellationToken.None);

        // Act
        sut.OnNext(new MapEvent(steamId, map));

        // Assert
        await tcs.Task;
        await mqttClient.Received(1).PublishAsync(
            Arg.Is<MqttMessage>(x =>
                x.Topic == $"{MqttConstants.BaseTopic}/{steamId}/map/status" &&
                x.Payload == "online" &&
                x.RetainFlag),
            Arg.Any<CancellationToken>());
    }

    [Theory, AutoNSubstituteData]
    public async Task Publishes_round_availability(
        [Frozen] IMqttClient mqttClient,
        SteamId64 steamId,
        Round round,
        AvailabilityMqttPublisher sut)
    {
        // Arrange
        using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));
        var tcs = new TaskCompletionSource<bool>();
        cts.Token.Register(() => tcs.TrySetCanceled(CancellationToken.None));
        mqttClient
            .When(x => x.PublishAsync(
                Arg.Is<MqttMessage>(m => m.Topic == $"{MqttConstants.BaseTopic}/{steamId}/round/status"),
                Arg.Any<CancellationToken>()))
            .Do(_ => tcs.SetResult(true));

        await sut.StartAsync(CancellationToken.None);

        // Act
        sut.OnNext(new RoundEvent(steamId, round));

        // Assert
        await tcs.Task;
        await mqttClient.Received(1).PublishAsync(
            Arg.Is<MqttMessage>(x =>
                x.Topic == $"{MqttConstants.BaseTopic}/{steamId}/round/status" &&
                x.Payload == "online" &&
                x.RetainFlag),
            Arg.Any<CancellationToken>());
    }

    [Theory, AutoNSubstituteData]
    public async Task Publishes_and_updates_availability_on_changes(
        [Frozen] IMqttClient mqttClient,
        SteamId64 steamId,
        Player player,
        AvailabilityMqttPublisher sut)
    {
        // Arrange
        using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));
        var tcs = new TaskCompletionSource<bool>();
        cts.Token.Register(() => tcs.TrySetCanceled(CancellationToken.None));
        var receivedTimes = 0;
        mqttClient
            .When(x => x.PublishAsync(
                Arg.Any<MqttMessage>(),
                Arg.Any<CancellationToken>()))
            .Do(_ =>
            {
                // We expect there to be a total of 4 MQTT messages published in this test.
                // 1: System online
                // 2: Player online
                // 3: Player offline
                // 4: Player online
                Interlocked.Increment(ref receivedTimes);
                if (receivedTimes == 4)
                {
                    tcs.SetResult(true);
                }
            });

        await sut.StartAsync(cts.Token);

        // Act
        sut.OnNext(new PlayerEvent(steamId, player));
        sut.OnNext(new PlayerEvent(steamId, Player: null));
        sut.OnNext(new PlayerEvent(steamId, player));

        // Assert
        await tcs.Task;

        await mqttClient.Received(2).PublishAsync(
            Arg.Is<MqttMessage>(x =>
                x.Topic == $"{MqttConstants.BaseTopic}/{steamId}/player/status" &&
                x.Payload == "online" &&
                x.RetainFlag),
            Arg.Any<CancellationToken>());

        await mqttClient.Received(1).PublishAsync(
            Arg.Is<MqttMessage>(x =>
                x.Topic == $"{MqttConstants.BaseTopic}/{steamId}/player/status" &&
                x.Payload == "offline" &&
                x.RetainFlag),
            Arg.Any<CancellationToken>());
    }

    [Theory, AutoNSubstituteData]
    public async Task Should_not_publish_same_availability_twice(
        [Frozen] IMqttClient mqttClient,
        SteamId64 steamId,
        Round round1,
        Round round2,
        AvailabilityMqttPublisher sut)
    {
        // Arrange
        using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));
        var tcs = new TaskCompletionSource<bool>();
        cts.Token.Register(() => tcs.TrySetCanceled(CancellationToken.None));
        mqttClient
            .When(x => x.PublishAsync(
                Arg.Is<MqttMessage>(m => m.Topic == $"{MqttConstants.BaseTopic}/{steamId}/round/status"),
                Arg.Any<CancellationToken>()))
            .Do(_ => tcs.SetResult(true));

        await sut.StartAsync(CancellationToken.None);

        // Act
        sut.OnNext(new RoundEvent(steamId, round1));
        sut.OnNext(new RoundEvent(steamId, round2));

        // Assert
        await tcs.Task;
        await mqttClient.Received(1).PublishAsync(
            Arg.Is<MqttMessage>(x =>
                x.Topic == $"{MqttConstants.BaseTopic}/{steamId}/round/status" &&
                x.Payload == "online" &&
                x.RetainFlag),
            Arg.Any<CancellationToken>());
    }
}