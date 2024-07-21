using System.Threading.Channels;
using LupusBytes.CS2.GameStateIntegration.Contracts;
using LupusBytes.CS2.GameStateIntegration.Events;
using Microsoft.Extensions.Hosting;

namespace LupusBytes.CS2.GameStateIntegration.Mqtt;

public class AvailabilityMqttPublisher(
    IGameStateService gameStateService,
    IMqttClient mqttClient) : BackgroundService,
    IObserver<PlayerEvent>,
    IObserver<PlayerStateEvent>,
    IObserver<MapEvent>,
    IObserver<RoundEvent>
{
    private static readonly BoundedChannelOptions ChannelOptions = new(1000)
    {
        SingleWriter = false,
        SingleReader = true,
        FullMode = BoundedChannelFullMode.DropOldest,
    };
    private readonly HashSet<SteamId64> onlinePlayers = [];
    private readonly HashSet<SteamId64> onlinePlayerStates = [];
    private readonly HashSet<SteamId64> onlineMaps = [];
    private readonly HashSet<SteamId64> onlineRounds = [];
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

    public void OnError(Exception error) => throw error;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var playerSubscription = gameStateService.Subscribe(this as IObserver<PlayerEvent>);
        using var playerStateSubscription = gameStateService.Subscribe(this as IObserver<PlayerStateEvent>);
        using var mapSubscription = gameStateService.Subscribe(this as IObserver<MapEvent>);
        using var roundStateSubscription = gameStateService.Subscribe(this as IObserver<RoundEvent>);

        await SetBaseAvailability(stoppingToken);

        var tasks = new[]
        {
            ProcessChannelAsync(playerChannel, onlinePlayers, ShouldBeOnline, "player/status", stoppingToken),
            ProcessChannelAsync(playerStateChannel, onlinePlayerStates, ShouldBeOnline, "player-state/status", stoppingToken),
            ProcessChannelAsync(mapChannel, onlineMaps, ShouldBeOnline, "map/status", stoppingToken),
            ProcessChannelAsync(roundChannel, onlineRounds, ShouldBeOnline, "round/status", stoppingToken),
        };

        // This task must be awaited to prevent the subscriptions from being disposed.
        await Task.WhenAll(tasks);
    }

    private Task SetBaseAvailability(CancellationToken cancellationToken) =>
        mqttClient.PublishAsync(
            new MqttMessage
            {
                Topic = "cs2mqtt/status",
                Payload = "online",
                RetainFlag = true,
            },
            cancellationToken);

    private async Task ProcessChannelAsync<TEvent>(
        Channel<TEvent> channel,
        HashSet<SteamId64> onlineSet,
        Func<TEvent, bool> shouldBeOnlineFunc,
        string topicSuffix,
        CancellationToken cancellationToken)
        where TEvent : BaseEvent
    {
        while (await channel.Reader.WaitToReadAsync(cancellationToken))
        {
            while (channel.Reader.TryRead(out var @event))
            {
                var isOnline = !onlineSet.Add(@event.SteamId);
                var shouldBeOnline = shouldBeOnlineFunc(@event);

                if (shouldBeOnline == isOnline)
                {
                    continue;
                }

                await mqttClient.PublishAsync(
                    new MqttMessage
                    {
                        Topic = $"cs2mqtt/{@event.SteamId}/{topicSuffix}",
                        Payload = shouldBeOnline ? "online" : "offline",
                        RetainFlag = true,
                    },
                    cancellationToken);

                if (!shouldBeOnline)
                {
                    onlineSet.Remove(@event.SteamId);
                }
            }
        }
    }

    private static bool ShouldBeOnline(PlayerEvent @event) => @event.Player is not null;
    private static bool ShouldBeOnline(PlayerStateEvent @event) => @event.PlayerState is not null;
    private static bool ShouldBeOnline(MapEvent @event) => @event.Map is not null;
    private static bool ShouldBeOnline(RoundEvent @event) => @event.Round is not null;
}