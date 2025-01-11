using System.Threading.Channels;
using LupusBytes.CS2.GameStateIntegration.Events;
using LupusBytes.CS2.GameStateIntegration.Mqtt.Extensions;

namespace LupusBytes.CS2.GameStateIntegration.Mqtt;

public sealed class GameStateMqttPublisher(
    IGameStateService gameStateService,
    IMqttClient mqttClient) : GameStateObserverService(gameStateService)
{
    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var tasks = new[]
        {
            ProcessChannelAsync(PlayerChannelReader, stoppingToken),
            ProcessChannelAsync(PlayerStateChannelReader, stoppingToken),
            ProcessChannelAsync(MapChannelReader, stoppingToken),
            ProcessChannelAsync(RoundChannelReader, stoppingToken),
        };

        return Task.WhenAll(tasks);
    }

    private async Task ProcessChannelAsync<TEvent>(ChannelReader<TEvent> channelReader, CancellationToken cancellationToken)
        where TEvent : BaseEvent
    {
        await foreach (var @event in channelReader.ReadAllAsync(cancellationToken))
        {
            await mqttClient.PublishAsync(@event.ToMqttMessage(), cancellationToken);
        }
    }
}