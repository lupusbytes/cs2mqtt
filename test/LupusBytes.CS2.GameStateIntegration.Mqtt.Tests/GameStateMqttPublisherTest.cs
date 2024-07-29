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