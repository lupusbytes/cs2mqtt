using System.Collections.Concurrent;
using System.Threading.Channels;
using LupusBytes.CS2.GameStateIntegration.Contracts;
using LupusBytes.CS2.GameStateIntegration.Events;

namespace LupusBytes.CS2.GameStateIntegration.Mqtt.HomeAssistant;

public sealed class HomeAssistantDevicePublisher(
    IGameStateService gameStateService,
    IMqttClient mqttClient) : GameStateObserverService(gameStateService)
{
    private const string BridgeDeviceId = $"{Constants.ProjectName}_bridge";
    private const string Manufacturer = "lupusbytes";

    private readonly ConcurrentDictionary<SteamId64, Device> devices = [];
    private readonly HashSet<SteamId64> publishedPlayerConfigs = [];
    private readonly HashSet<SteamId64> publishedPlayerStateConfigs = [];
    private readonly HashSet<SteamId64> publishedMapConfigs = [];
    private readonly HashSet<SteamId64> publishedRoundConfigs = [];

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await PublishBridgeDeviceAsync(stoppingToken);
        await ProcessChannelsAsync(stoppingToken);
    }

    private Task ProcessChannelsAsync(CancellationToken stoppingToken)
        => Task.WhenAll(
            ProcessChannelAsync(
                PlayerChannelReader,
                publishedPlayerConfigs,
                device => new PlayerDiscoveryMessages(device),
                stoppingToken),
            ProcessChannelAsync(
                PlayerStateChannelReader,
                publishedPlayerStateConfigs,
                device => new PlayerStateDiscoveryMessages(device),
                stoppingToken),
            ProcessChannelAsync(
                MapChannelReader,
                publishedMapConfigs,
                device => new MapDiscoveryMessages(device),
                stoppingToken),
            ProcessChannelAsync(
                RoundChannelReader,
                publishedRoundConfigs,
                device => new RoundDiscoveryMessages(device),
                stoppingToken));

    private async Task ProcessChannelAsync<TEvent>(
        ChannelReader<TEvent> channelReader,
        HashSet<SteamId64> publishedConfigSet,
        Func<Device, MqttDiscoveryMessages> discoveryMessages,
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

                foreach (var discoveryMessage in discoveryMessages(device))
                {
                    await mqttClient.PublishAsync(discoveryMessage, cancellationToken);
                }
            }
        }
    }

    private static Device CreateDevice(SteamId64 steamId) => new(
        Id: steamId.ToString(),
        Name: $"CS2 {steamId.ToTextualString()}",
        Manufacturer: Manufacturer,
        Model: Constants.ProjectName,
        SoftwareVersion: Constants.Version,
        ViaDevice: BridgeDeviceId);

    private async Task PublishBridgeDeviceAsync(CancellationToken cancellationToken)
    {
        var bridgeDevice = new Device(
            Id: BridgeDeviceId,
            Name: "CS2 MQTT Bridge",
            Manufacturer: Manufacturer,
            Model: Constants.ProjectName,
            SoftwareVersion: Constants.Version);

        var discoveryMessages = new BridgeDiscoveryMessages(bridgeDevice);

        foreach (var discoveryMessage in discoveryMessages)
        {
            await mqttClient.PublishAsync(discoveryMessage, cancellationToken);
        }
    }
}