using System.Threading.Channels;
using LupusBytes.CS2.GameStateIntegration.Events;
using LupusBytes.CS2.GameStateIntegration.Mqtt.Extensions;
using Microsoft.Extensions.Hosting;

namespace LupusBytes.CS2.GameStateIntegration.Mqtt;

public sealed class GameStateMqttPublisher(
    IGameStateService gameStateService,
    IMqttClient mqttClient) : BackgroundService,
    IObserver<MapEvent>,
    IObserver<PlayerEvent>,
    IObserver<PlayerStateEvent>,
    IObserver<RoundEvent>
{
    public const string BaseTopic = "cs2mqtt";

    private readonly Channel<BaseEvent> channel = Channel.CreateBounded<BaseEvent>(new BoundedChannelOptions(1000)
    {
        SingleWriter = false,
        SingleReader = true,
        FullMode = BoundedChannelFullMode.DropOldest,
    });

    public void OnNext(MapEvent value) => channel.Writer.TryWrite(value);
    public void OnNext(RoundEvent value) => channel.Writer.TryWrite(value);
    public void OnNext(PlayerEvent value) => channel.Writer.TryWrite(value);
    public void OnNext(PlayerStateEvent value) => channel.Writer.TryWrite(value);

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var playerSubscription = gameStateService.Subscribe(this as IObserver<PlayerEvent>);
        using var playerStateSubscription = gameStateService.Subscribe(this as IObserver<PlayerStateEvent>);
        using var roundSubscription = gameStateService.Subscribe(this as IObserver<RoundEvent>);
        using var mapSubscription = gameStateService.Subscribe(this as IObserver<MapEvent>);

        while (await channel.Reader.WaitToReadAsync(stoppingToken))
        {
            while (channel.Reader.TryRead(out var @event))
            {
                await mqttClient.PublishAsync(@event.ToMqttMessage(), stoppingToken);
            }
        }
    }

    public void OnCompleted()
    {
    }

    public void OnError(Exception error)
    {
    }
}