using System.Collections.Concurrent;
using System.Text.Json;
using System.Threading.Channels;
using LupusBytes.CS2.GameStateIntegration.Contracts;
using LupusBytes.CS2.GameStateIntegration.Events;
using Microsoft.Extensions.Hosting;

namespace LupusBytes.CS2.GameStateIntegration.Mqtt.HomeAssistant;

public sealed class HomeAssistantDevicePublisher(
    IGameStateService gameStateService,
    IMqttClient mqttClient) :
    BackgroundService,
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
    private readonly ConcurrentDictionary<SteamId64, Device> devices = [];
    private readonly HashSet<SteamId64> publishedPlayerConfigs = [];
    private readonly HashSet<SteamId64> publishedPlayerStateConfigs = [];
    private readonly HashSet<SteamId64> publishedMapConfigs = [];
    private readonly HashSet<SteamId64> publishedRoundConfigs = [];
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
        using var roundSubscription = gameStateService.Subscribe(this as IObserver<RoundEvent>);
        using var mapSubscription = gameStateService.Subscribe(this as IObserver<MapEvent>);

        var tasks = new[]
        {
            ProcessChannelAsync(playerChannel, publishedPlayerConfigs, stoppingToken),
            ProcessChannelAsync(playerStateChannel, publishedPlayerStateConfigs, stoppingToken),
            ProcessChannelAsync(mapChannel, publishedMapConfigs, stoppingToken),
            ProcessChannelAsync(roundChannel, publishedRoundConfigs, stoppingToken),
        };

        // This task must be awaited to prevent the subscriptions from being disposed.
        await Task.WhenAll(tasks);
    }

    private async Task ProcessChannelAsync<TEvent>(
        Channel<TEvent> channel,
        HashSet<SteamId64> publishedConfigSet,
        CancellationToken cancellationToken)
        where TEvent : BaseEvent
    {
        while (await channel.Reader.WaitToReadAsync(cancellationToken))
        {
            while (channel.Reader.TryRead(out var @event))
            {
                var device = devices.GetOrAdd(@event.SteamId, CreateDevice);

                if (!publishedConfigSet.Add(@event.SteamId))
                {
                    continue;
                }

                var sensors = CreateDeviceSensors(@event, device);

                await SendDiscoveryPayloadsAsync(sensors, cancellationToken);
            }
        }
    }

    private async Task SendDiscoveryPayloadsAsync(
        IDeviceSensors deviceSensors,
        CancellationToken cancellationToken)
    {
        foreach (var discoveryPayload in deviceSensors.DiscoveryPayloads)
        {
            var message = new MqttMessage
            {
                Topic = $"homeassistant/sensor/{discoveryPayload.UniqueId}/config",
                Payload = JsonSerializer.Serialize(discoveryPayload),
            };

            await mqttClient.PublishAsync(message, cancellationToken);
        }
    }

    private static Device CreateDevice(SteamId64 steamId) => new(
        Id: steamId.ToString(),
        Name: "CS2",
        Manufacturer: "lupusbytes",
        Model: "cs2mqtt",
        SoftwareVersion: "0.0.1-beta");

    private static IDeviceSensors CreateDeviceSensors(BaseEvent @event, Device device) => @event switch
    {
        PlayerEvent => new PlayerSensors(device),
        PlayerStateEvent => new PlayerStateSensors(device),
        MapEvent => new MapSensors(device),
        RoundEvent => new RoundSensors(device),
        _ => throw new ArgumentException($"Unknown event type {@event.GetType()}", nameof(@event)),
    };
}