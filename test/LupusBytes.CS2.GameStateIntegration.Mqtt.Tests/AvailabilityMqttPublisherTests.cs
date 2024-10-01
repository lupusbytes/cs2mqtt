using System.Collections.ObjectModel;
using LupusBytes.CS2.GameStateIntegration.Contracts;
using LupusBytes.CS2.GameStateIntegration.Events;
using LupusBytes.CS2.GameStateIntegration.Extensions;

namespace LupusBytes.CS2.GameStateIntegration.Mqtt.Tests;

public class AvailabilityMqttPublisherTests
{
    [Theory, AutoNSubstituteData]
    public async Task Publishes_system_availability_on_startup(
        [Frozen] IMqttClient mqttClient,
        AvailabilityMqttPublisher sut)
    {
        // Act
        await sut.StartAsync(CancellationToken.None);

        // Assert
        await AssertAvailabilityPublishedOnTopic(
            mqttClient,
            MqttConstants.SystemAvailabilityTopic,
            payload: "online");
    }

    [Theory, AutoNSubstituteData]
    public async Task Publishes_system_availability_on_shutdown(
        [Frozen] IMqttClient mqttClient,
        AvailabilityMqttPublisher sut)
    {
        // Act
        await sut.StopAsync(CancellationToken.None);

        // Assert
        await AssertAvailabilityPublishedOnTopic(
            mqttClient,
            MqttConstants.SystemAvailabilityTopic,
            payload: "offline");
    }

    [Theory, AutoNSubstituteData]
    public async Task Publishes_player_availability(
        [Frozen] IMqttClient mqttClient,
        SteamId64 steamId,
        Player player,
        AvailabilityMqttPublisher sut)
    {
        // Arrange
        var topic = $"{MqttConstants.BaseTopic}/{steamId}/player/status";
        var tcs = TaskHelper.CompletionSourceFromTopicPublishment(mqttClient, topic);
        using var cts = TaskHelper.EnableCompletionSourceTimeout(tcs);
        await sut.StartAsync(CancellationToken.None);

        // Act
        sut.OnNext(new PlayerEvent(steamId, player));

        // Assert
        await tcs.Task;
        await AssertAvailabilityPublishedOnTopic(mqttClient, topic);
    }

    [Theory, AutoNSubstituteData]
    public async Task Publishes_player_state_availability(
        [Frozen] IMqttClient mqttClient,
        SteamId64 steamId,
        PlayerState playerState,
        AvailabilityMqttPublisher sut)
    {
        // Arrange
        var topic = $"{MqttConstants.BaseTopic}/{steamId}/player-state/status";
        var tcs = TaskHelper.CompletionSourceFromTopicPublishment(mqttClient, topic);
        using var cts = TaskHelper.EnableCompletionSourceTimeout(tcs);
        await sut.StartAsync(CancellationToken.None);

        // Act
        sut.OnNext(new PlayerStateEvent(steamId, playerState));

        // Assert
        await tcs.Task;
        await AssertAvailabilityPublishedOnTopic(mqttClient, topic);
    }

    [Theory, AutoNSubstituteData]
    public async Task Publishes_map_availability(
        [Frozen] IMqttClient mqttClient,
        SteamId64 steamId,
        Map map,
        AvailabilityMqttPublisher sut)
    {
        // Arrange
        var topic = $"{MqttConstants.BaseTopic}/{steamId}/map/status";
        var tcs = TaskHelper.CompletionSourceFromTopicPublishment(mqttClient, topic);
        using var cts = TaskHelper.EnableCompletionSourceTimeout(tcs);
        await sut.StartAsync(CancellationToken.None);

        // Act
        sut.OnNext(new MapEvent(steamId, map));

        // Assert
        await tcs.Task;
        await AssertAvailabilityPublishedOnTopic(mqttClient, topic);
    }

    [Theory, AutoNSubstituteData]
    public async Task Publishes_round_availability(
        [Frozen] IMqttClient mqttClient,
        SteamId64 steamId,
        Round round,
        AvailabilityMqttPublisher sut)
    {
        // Arrange
        var topic = $"{MqttConstants.BaseTopic}/{steamId}/round/status";
        var tcs = TaskHelper.CompletionSourceFromTopicPublishment(mqttClient, topic);
        using var cts = TaskHelper.EnableCompletionSourceTimeout(tcs);
        await sut.StartAsync(CancellationToken.None);

        // Act
        sut.OnNext(new RoundEvent(steamId, round));

        // Assert
        await tcs.Task;
        await AssertAvailabilityPublishedOnTopic(mqttClient, topic);
    }

    [Theory, AutoNSubstituteData]
    public async Task Publishes_and_updates_availability_on_changes(
        [Frozen] IMqttClient mqttClient,
        SteamId64 steamId,
        Player player,
        AvailabilityMqttPublisher sut)
    {
        // Arrange
        var topic = $"{MqttConstants.BaseTopic}/{steamId}/player/status";
        var tcs = new TaskCompletionSource<bool>();
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

        using var cts = TaskHelper.EnableCompletionSourceTimeout(tcs);
        await sut.StartAsync(CancellationToken.None);

        // Act
        sut.OnNext(new PlayerEvent(steamId, player));
        sut.OnNext(new PlayerEvent(steamId, Player: null));
        sut.OnNext(new PlayerEvent(steamId, player));

        // Assert
        await tcs.Task;

        await AssertAvailabilityPublishedOnTopic(
            mqttClient,
            topic,
            publishCount: 2,
            payload: "online");

        await AssertAvailabilityPublishedOnTopic(
            mqttClient,
            topic,
            publishCount: 1,
            payload: "offline");
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
        var topic = $"{MqttConstants.BaseTopic}/{steamId}/round/status";
        var tcs = TaskHelper.CompletionSourceFromTopicPublishment(mqttClient, topic);
        using var cts = TaskHelper.EnableCompletionSourceTimeout(tcs);
        await sut.StartAsync(CancellationToken.None);

        // Act
        sut.OnNext(new RoundEvent(steamId, round1));
        sut.OnNext(new RoundEvent(steamId, round2));

        // Assert
        await tcs.Task;
        await AssertAvailabilityPublishedOnTopic(
            mqttClient,
            topic,
            publishCount: 1);
    }

    [Theory, AutoNSubstituteData]
    public async Task Publishes_availability_on_all_provider_topics_on_shutdown(
        [Frozen] IMqttClient mqttClient,
        ReadOnlyCollection<GameStateData> gameStates,
        AvailabilityMqttPublisher sut)
    {
        // Arrange
        await sut.StartAsync(CancellationToken.None);

        var topics = new List<string>();
        foreach (var gameState in gameStates)
        {
            var steamId = gameState.Provider!.SteamId64;
            topics.Add($"{MqttConstants.BaseTopic}/{steamId}/player/status");
            topics.Add($"{MqttConstants.BaseTopic}/{steamId}/player-state/status");
            topics.Add($"{MqttConstants.BaseTopic}/{steamId}/map/status");
            topics.Add($"{MqttConstants.BaseTopic}/{steamId}/round/status");

            sut.OnNext(gameState.Player.ToEvent(steamId));
            sut.OnNext(gameState.Player!.State.ToEvent(steamId));
            sut.OnNext(gameState.Map.ToEvent(steamId));
            sut.OnNext(gameState.Round.ToEvent(steamId));
        }

        // Act
        await sut.StopAsync(CancellationToken.None);

        // Assert
        foreach (var topic in topics)
        {
            await AssertAvailabilityPublishedOnTopic(mqttClient, topic, payload: "offline");
        }
    }

    private static Task AssertAvailabilityPublishedOnTopic(
        IMqttClient mqttClient,
        string topic,
        int publishCount = 1,
        string payload = "online",
        bool retainFlag = true)
        => mqttClient
            .Received(publishCount)
            .PublishAsync(
                Arg.Is<MqttMessage>(x =>
                    x.Topic == topic &&
                    x.Payload == payload &&
                    x.RetainFlag == retainFlag),
                Arg.Any<CancellationToken>());
}