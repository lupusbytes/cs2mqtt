using System.Text.Json;
using LupusBytes.CS2.GameStateIntegration.Contracts;
using LupusBytes.CS2.GameStateIntegration.Events;

namespace LupusBytes.CS2.GameStateIntegration.Mqtt.Tests;

public class GameStateMqttPublisherTest
{
    [Theory, AutoNSubstituteData]
    public async Task Publishes_Player_data(
        [Frozen] IMqttClient mqttClient,
        SteamId64 steamId,
        Player player,
        GameStateMqttPublisher sut)
    {
        // Arrange
        var topic = $"{MqttConstants.BaseTopic}/{steamId}/player";
        var tcs = TaskHelper.CompletionSourceFromTopicPublishment(mqttClient, topic);
        using var cts = TaskHelper.EnableCompletionSourceTimeout(tcs);
        await sut.StartAsync(CancellationToken.None);

        // Act
        sut.OnNext(new PlayerEvent(steamId, player));

        // Assert
        await tcs.Task;
        var expectedPayload = JsonSerializer.Serialize(player);
        await AssertPayloadPublishedOnTopic(mqttClient, topic, expectedPayload);
    }

    [Theory, AutoNSubstituteData]
    public async Task Publishes_PlayerState_data(
        [Frozen] IMqttClient mqttClient,
        SteamId64 steamId,
        PlayerState playerState,
        GameStateMqttPublisher sut)
    {
        // Arrange
        var topic = $"{MqttConstants.BaseTopic}/{steamId}/player-state";
        var tcs = TaskHelper.CompletionSourceFromTopicPublishment(mqttClient, topic);
        using var cts = TaskHelper.EnableCompletionSourceTimeout(tcs);
        await sut.StartAsync(CancellationToken.None);

        // Act
        sut.OnNext(new PlayerStateEvent(steamId, playerState));

        // Assert
        await tcs.Task;
        var expectedPayload = JsonSerializer.Serialize(playerState);
        await AssertPayloadPublishedOnTopic(mqttClient, topic, expectedPayload);
    }

    [Theory, AutoNSubstituteData]
    public async Task Publishes_Map_data(
        [Frozen] IMqttClient mqttClient,
        SteamId64 steamId,
        Map map,
        GameStateMqttPublisher sut)
    {
        // Arrange
        var topic = $"{MqttConstants.BaseTopic}/{steamId}/map";
        var tcs = TaskHelper.CompletionSourceFromTopicPublishment(mqttClient, topic);
        using var cts = TaskHelper.EnableCompletionSourceTimeout(tcs);
        await sut.StartAsync(CancellationToken.None);

        // Act
        sut.OnNext(new MapEvent(steamId, map));

        // Assert
        await tcs.Task;
        var expectedPayload = JsonSerializer.Serialize(map);
        await AssertPayloadPublishedOnTopic(mqttClient, topic, expectedPayload);
    }

    [Theory, AutoNSubstituteData]
    public async Task Publishes_Round_data(
        [Frozen] IMqttClient mqttClient,
        SteamId64 steamId,
        Round round,
        GameStateMqttPublisher sut)
    {
        // Arrange
        var topic = $"{MqttConstants.BaseTopic}/{steamId}/round";
        var tcs = TaskHelper.CompletionSourceFromTopicPublishment(mqttClient, topic);
        using var cts = TaskHelper.EnableCompletionSourceTimeout(tcs);
        await sut.StartAsync(CancellationToken.None);

        // Act
        sut.OnNext(new RoundEvent(steamId, round));

        // Assert
        await tcs.Task;
        var expectedPayload = JsonSerializer.Serialize(round);
        await AssertPayloadPublishedOnTopic(mqttClient, topic, expectedPayload);
    }

    private static Task AssertPayloadPublishedOnTopic(
        IMqttClient mqttClient,
        string topic,
        string payload,
        bool retainFlag = true,
        int publishCount = 1)
        => mqttClient
            .Received(publishCount)
            .PublishAsync(
                Arg.Is<MqttMessage>(x =>
                    x.Topic == topic &&
                    x.Payload == payload &&
                    x.RetainFlag == retainFlag),
                Arg.Any<CancellationToken>());
}