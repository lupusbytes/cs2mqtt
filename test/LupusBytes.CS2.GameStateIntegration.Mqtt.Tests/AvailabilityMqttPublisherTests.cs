using System.Collections.ObjectModel;
using LupusBytes.CS2.GameStateIntegration.Contracts;

namespace LupusBytes.CS2.GameStateIntegration.Mqtt.Tests;

public class AvailabilityMqttPublisherTests
{
    [Theory, AutoNSubstituteData]
    public async Task Publishes_system_availability_on_startup(
        [Frozen] IMqttClient mqttClient,
        AvailabilityMqttPublisher sut)
    {
        // Act
        await sut.StartAsync(TestContext.Current.CancellationToken);

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
        await sut.StopAsync(TestContext.Current.CancellationToken);

        // Assert
        await AssertAvailabilityPublishedOnTopic(
            mqttClient,
            MqttConstants.SystemAvailabilityTopic,
            payload: "offline");
    }

    [Theory, AutoNSubstituteData]
    public async Task Publishes_player_availability(
        [Frozen] IMqttClient mqttClient,
        [Frozen] IGameStateService gameStateService,
        SteamId64 steamId,
        Player player,
        AvailabilityMqttPublisher sut)
    {
        // Arrange
        var topic = $"{MqttConstants.BaseTopic}/{steamId}/player/status";
        var tcs = TaskHelper.CompletionSourceFromTopicPublishment(mqttClient, topic);
        await sut.StartAsync(TestContext.Current.CancellationToken);

        // Act
        gameStateService.PlayerUpdated += Raise.EventWith(new StateUpdateEventArgs<Player>(steamId, player));

        // Assert
        await TaskHelper.WaitForCompletionAsync(tcs);
        await AssertAvailabilityPublishedOnTopic(mqttClient, topic);
    }

    [Theory, AutoNSubstituteData]
    public async Task Publishes_player_as_online_after_player_was_initially_offline(
        [Frozen] IMqttClient mqttClient,
        [Frozen] IGameStateService gameStateService,
        SteamId64 steamId,
        Player player,
        AvailabilityMqttPublisher sut)
    {
        // Arrange
        var topic = $"{MqttConstants.BaseTopic}/{steamId}/player/status";
        var tcs = new TaskCompletionSource<bool>();
        mqttClient
            .When(x => x.PublishAsync(
                Arg.Is<MqttMessage>(m => m.Topic == topic && m.Payload == "online"),
                Arg.Any<CancellationToken>()))
            .Do(_ => tcs.SetResult(true));

        await sut.StartAsync(TestContext.Current.CancellationToken);

        // Act
        gameStateService.PlayerUpdated += Raise.EventWith(new StateUpdateEventArgs<Player>(steamId, state: null));
        gameStateService.PlayerUpdated += Raise.EventWith(new StateUpdateEventArgs<Player>(steamId, player));

        // Assert
        await TaskHelper.WaitForCompletionAsync(tcs);
        await AssertAvailabilityPublishedOnTopic(mqttClient, topic);
    }

    [Theory, AutoNSubstituteData]
    public async Task Publishes_player_state_availability(
        [Frozen] IMqttClient mqttClient,
        [Frozen] IGameStateService gameStateService,
        SteamId64 steamId,
        PlayerState playerState,
        AvailabilityMqttPublisher sut)
    {
        // Arrange
        var topic = $"{MqttConstants.BaseTopic}/{steamId}/player-state/status";
        var tcs = TaskHelper.CompletionSourceFromTopicPublishment(mqttClient, topic);
        await sut.StartAsync(TestContext.Current.CancellationToken);

        // Act
        gameStateService.PlayerStateUpdated += Raise.EventWith(new StateUpdateEventArgs<PlayerState>(steamId, playerState));

        // Assert
        await TaskHelper.WaitForCompletionAsync(tcs);
        await AssertAvailabilityPublishedOnTopic(mqttClient, topic);
    }

    [Theory, AutoNSubstituteData]
    public async Task Publishes_player_match_stats_availability(
        [Frozen] IMqttClient mqttClient,
        [Frozen] IGameStateService gameStateService,
        SteamId64 steamId,
        PlayerMatchStats playerMatchStats,
        AvailabilityMqttPublisher sut)
    {
        // Arrange
        var topic = $"{MqttConstants.BaseTopic}/{steamId}/player-match-stats/status";
        var tcs = TaskHelper.CompletionSourceFromTopicPublishment(mqttClient, topic);
        await sut.StartAsync(TestContext.Current.CancellationToken);

        // Act
        gameStateService.PlayerMatchStatsUpdated += Raise.EventWith(new StateUpdateEventArgs<PlayerMatchStats>(steamId, playerMatchStats));

        // Assert
        await TaskHelper.WaitForCompletionAsync(tcs);
        await AssertAvailabilityPublishedOnTopic(mqttClient, topic);
    }

    [Theory, AutoNSubstituteData]
    public async Task Publishes_map_availability(
        [Frozen] IMqttClient mqttClient,
        [Frozen] IGameStateService gameStateService,
        SteamId64 steamId,
        Map map,
        AvailabilityMqttPublisher sut)
    {
        // Arrange
        var topic = $"{MqttConstants.BaseTopic}/{steamId}/map/status";
        var tcs = TaskHelper.CompletionSourceFromTopicPublishment(mqttClient, topic);
        await sut.StartAsync(TestContext.Current.CancellationToken);

        // Act
        gameStateService.MapUpdated += Raise.EventWith(new StateUpdateEventArgs<Map>(steamId, map));

        // Assert
        await TaskHelper.WaitForCompletionAsync(tcs);
        await AssertAvailabilityPublishedOnTopic(mqttClient, topic);
    }

    [Theory, AutoNSubstituteData]
    public async Task Publishes_round_availability(
        [Frozen] IMqttClient mqttClient,
        [Frozen] IGameStateService gameStateService,
        SteamId64 steamId,
        Round round,
        AvailabilityMqttPublisher sut)
    {
        // Arrange
        var topic = $"{MqttConstants.BaseTopic}/{steamId}/round/status";
        var tcs = TaskHelper.CompletionSourceFromTopicPublishment(mqttClient, topic);
        await sut.StartAsync(TestContext.Current.CancellationToken);

        // Act
        gameStateService.RoundUpdated += Raise.EventWith(new StateUpdateEventArgs<Round>(steamId, round));

        // Assert
        await TaskHelper.WaitForCompletionAsync(tcs);
        await AssertAvailabilityPublishedOnTopic(mqttClient, topic);
    }

    [Theory, AutoNSubstituteData]
    public async Task Publishes_and_updates_availability_on_changes(
        [Frozen] IMqttClient mqttClient,
        [Frozen] IGameStateService gameStateService,
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

        await sut.StartAsync(TestContext.Current.CancellationToken);

        // Act
        gameStateService.PlayerUpdated += Raise.EventWith(new StateUpdateEventArgs<Player>(steamId, player));
        gameStateService.PlayerUpdated += Raise.EventWith(new StateUpdateEventArgs<Player>(steamId, state: null));
        gameStateService.PlayerUpdated += Raise.EventWith(new StateUpdateEventArgs<Player>(steamId, player));

        // Assert
        await TaskHelper.WaitForCompletionAsync(tcs);

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
        [Frozen] IGameStateService gameStateService,
        SteamId64 steamId,
        Round round1,
        Round round2,
        AvailabilityMqttPublisher sut)
    {
        // Arrange
        var topic = $"{MqttConstants.BaseTopic}/{steamId}/round/status";
        var tcs = TaskHelper.CompletionSourceFromTopicPublishment(mqttClient, topic);
        await sut.StartAsync(TestContext.Current.CancellationToken);

        // Act
        gameStateService.RoundUpdated += Raise.EventWith(new StateUpdateEventArgs<Round>(steamId, round1));
        gameStateService.RoundUpdated += Raise.EventWith(new StateUpdateEventArgs<Round>(steamId, round2));

        // Assert
        await TaskHelper.WaitForCompletionAsync(tcs);
        await AssertAvailabilityPublishedOnTopic(
            mqttClient,
            topic,
            publishCount: 1);
    }

    [Theory, AutoNSubstituteData]
    public async Task Publishes_availability_on_all_provider_topics_on_shutdown(
        [Frozen] IMqttClient mqttClient,
        [Frozen] IGameStateService gameStateService,
        ReadOnlyCollection<GameStateData> gameStates,
        AvailabilityMqttPublisher sut)
    {
        // Arrange
        await sut.StartAsync(TestContext.Current.CancellationToken);

        var topics = gameStates
            .Select(x => x.Provider!.SteamId64)
            .SelectMany(steamId => new[]
            {
                $"{MqttConstants.BaseTopic}/{steamId}/player/status",
                $"{MqttConstants.BaseTopic}/{steamId}/player-state/status",
                $"{MqttConstants.BaseTopic}/{steamId}/player-match-stats/status",
                $"{MqttConstants.BaseTopic}/{steamId}/map/status",
                $"{MqttConstants.BaseTopic}/{steamId}/round/status",
            })
            .ToList();

        var tcs = TaskHelper.CompletionSourceFromTopicPublishment(mqttClient, topics);

        foreach (var gameState in gameStates)
        {
            var steamId = gameState.Provider!.SteamId64;
            gameStateService.PlayerUpdated += Raise.EventWith(new StateUpdateEventArgs<Player>(steamId, gameState.Player));
            gameStateService.PlayerStateUpdated += Raise.EventWith(new StateUpdateEventArgs<PlayerState>(steamId, gameState.Player!.State));
            gameStateService.PlayerMatchStatsUpdated += Raise.EventWith(new StateUpdateEventArgs<PlayerMatchStats>(steamId, gameState.Player.MatchStats));
            gameStateService.MapUpdated += Raise.EventWith(new StateUpdateEventArgs<Map>(steamId, gameState.Map));
            gameStateService.RoundUpdated += Raise.EventWith(new StateUpdateEventArgs<Round>(steamId, gameState.Round));
        }

        await TaskHelper.WaitForCompletionAsync(tcs);

        // Act
        await sut.StopAsync(TestContext.Current.CancellationToken);

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