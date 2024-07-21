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

    private static readonly BoundedChannelOptions ChannelOptions = new(1000)
    {
        SingleWriter = false,
        SingleReader = true,
        FullMode = BoundedChannelFullMode.DropOldest,
    };

    private readonly Channel<PlayerEvent> playerChannel = Channel.CreateBounded<PlayerEvent>(ChannelOptions);
    private readonly Channel<PlayerStateEvent> playerStateChannel = Channel.CreateBounded<PlayerStateEvent>(ChannelOptions);
    private readonly Channel<MapEvent> mapChannel = Channel.CreateBounded<MapEvent>(ChannelOptions);
    private readonly Channel<RoundEvent> roundChannel = Channel.CreateBounded<RoundEvent>(ChannelOptions);

    public void OnNext(PlayerEvent value) => playerChannel.Writer.TryWrite(value);
    public void OnNext(PlayerStateEvent value) => playerStateChannel.Writer.TryWrite(value);
    public void OnNext(MapEvent value) => mapChannel.Writer.TryWrite(value);
    public void OnNext(RoundEvent value) => roundChannel.Writer.TryWrite(value);

    public void OnCompleted()
    {
    }

    public void OnError(Exception error)
    {
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var playerSubscription = gameStateService.Subscribe(this as IObserver<PlayerEvent>);
        using var playerStateSubscription = gameStateService.Subscribe(this as IObserver<PlayerStateEvent>);
        using var roundSubscription = gameStateService.Subscribe(this as IObserver<RoundEvent>);
        using var mapSubscription = gameStateService.Subscribe(this as IObserver<MapEvent>);

        var tasks = new[]
        {
            ProcessChannelAsync(playerChannel, stoppingToken),
            ProcessChannelAsync(playerStateChannel, stoppingToken),
            ProcessChannelAsync(mapChannel, stoppingToken),
            ProcessChannelAsync(roundChannel, stoppingToken),
        };

        // This task must be awaited to prevent the subscriptions from being disposed.
        await Task.WhenAll(tasks);
    }

    private async Task ProcessChannelAsync<TEvent>(Channel<TEvent> channel, CancellationToken cancellationToken)
        where TEvent : BaseEvent
    {
        while (await channel.Reader.WaitToReadAsync(cancellationToken))
        {
            while (channel.Reader.TryRead(out var @event))
            {
                await mqttClient.PublishAsync(@event.ToMqttMessage(), cancellationToken);
            }
        }
    }
}