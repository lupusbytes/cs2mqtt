using System.Collections.Concurrent;
using System.Text.Json;
using System.Threading.Channels;
using LupusBytes.CS2.GameStateIntegration.Contracts;
using LupusBytes.CS2.GameStateIntegration.Events;

namespace LupusBytes.CS2.GameStateIntegration.Mqtt.HomeAssistant;

public sealed class HomeAssistantDevicePublisher(
    IGameStateService gameStateService,
    IMqttClient mqttClient) : GameStateObserverService(gameStateService)
{
    private readonly ConcurrentDictionary<SteamId64, Device> devices = [];
    private readonly HashSet<SteamId64> publishedPlayerConfigs = [];
    private readonly HashSet<SteamId64> publishedPlayerStateConfigs = [];
    private readonly HashSet<SteamId64> publishedMapConfigs = [];
    private readonly HashSet<SteamId64> publishedRoundConfigs = [];

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
        => Task.WhenAll(
            ProcessChannelAsync(PlayerChannelReader, publishedPlayerConfigs, stoppingToken),
            ProcessChannelAsync(PlayerStateChannelReader, publishedPlayerStateConfigs, stoppingToken),
            ProcessChannelAsync(MapChannelReader, publishedMapConfigs, stoppingToken),
            ProcessChannelAsync(RoundChannelReader, publishedRoundConfigs, stoppingToken));

    private async Task ProcessChannelAsync<TEvent>(
        ChannelReader<TEvent> channelReader,
        HashSet<SteamId64> publishedConfigSet,
        CancellationToken cancellationToken)
        where TEvent : BaseEvent
    {
        while (await channelReader.WaitToReadAsync(cancellationToken))
        {
            while (channelReader.TryRead(out var @event))
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
                RetainFlag = true,
            };

            await mqttClient.PublishAsync(message, cancellationToken);
        }
    }

    private static Device CreateDevice(SteamId64 steamId) => new(
        Id: steamId.ToString(),
        Name: "CS2",
        Manufacturer: "lupusbytes",
        Model: Constants.ProjectName,
        SoftwareVersion: Constants.Version);

    private static IDeviceSensors CreateDeviceSensors(BaseEvent @event, Device device) => @event switch
    {
        PlayerEvent => new PlayerSensors(device),
        PlayerStateEvent => new PlayerStateSensors(device),
        MapEvent => new MapSensors(device),
        RoundEvent => new RoundSensors(device),
        _ => throw new ArgumentException($"Unknown event type {@event.GetType()}", nameof(@event)),
    };
}