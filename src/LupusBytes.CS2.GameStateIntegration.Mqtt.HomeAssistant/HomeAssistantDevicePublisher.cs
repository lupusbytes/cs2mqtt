using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using LupusBytes.CS2.GameStateIntegration.Contracts;

namespace LupusBytes.CS2.GameStateIntegration.Mqtt.HomeAssistant;

public sealed class HomeAssistantDevicePublisher : GameStateSubscriberService
{
    private const string BridgeDeviceId = $"{Constants.ProjectName}_bridge";
    private const string Manufacturer = "lupusbytes";

    private readonly ConcurrentDictionary<SteamId64, Device> devices = [];
    private readonly HashSet<SteamId64> publishedProviderConfigs = [];
    private readonly HashSet<SteamId64> publishedPlayerConfigs = [];
    private readonly HashSet<SteamId64> publishedPlayerStateConfigs = [];
    private readonly HashSet<SteamId64> publishedPlayerMatchStatsConfigs = [];
    private readonly HashSet<SteamId64> publishedMapConfigs = [];
    private readonly HashSet<SteamId64> publishedRoundConfigs = [];

    private readonly IMqttClient mqttClient;

    public HomeAssistantDevicePublisher(IGameStateService gameStateService, IMqttClient mqttClient)
        : base(gameStateService)
    {
        this.mqttClient = mqttClient;

        SubscribeToProvider((e, ct) => PublishDiscoveryMessagesAsync(e, publishedProviderConfigs, device => new ProviderDiscoveryMessages(device), ct));
        SubscribeToPlayer((e, ct) => PublishDiscoveryMessagesAsync(e, publishedPlayerConfigs, device => new PlayerDiscoveryMessages(device), ct));
        SubscribeToPlayerState((e, ct) => PublishDiscoveryMessagesAsync(e, publishedPlayerStateConfigs, device => new PlayerStateDiscoveryMessages(device), ct));
        SubscribeToPlayerMatchStats((e, ct) => PublishDiscoveryMessagesAsync(e, publishedPlayerMatchStatsConfigs, device => new PlayerMatchStatsDiscoveryMessages(device), ct));
        SubscribeToMap((e, ct) => PublishDiscoveryMessagesAsync(e, publishedMapConfigs, device => new MapDiscoveryMessages(device), ct));
        SubscribeToRound((e, ct) => PublishDiscoveryMessagesAsync(e, publishedRoundConfigs, device => new RoundDiscoveryMessages(device), ct));
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await PublishBridgeDeviceAsync(stoppingToken);
        await base.ExecuteAsync(stoppingToken);
    }

    private static Device CreateDevice(SteamId64 steamId) => new(
        Id: steamId.ToString(),
        Name: $"CS2 {steamId.ToTextualString()}",
        Manufacturer: Manufacturer,
        Model: Constants.ProjectName,
        SoftwareVersion: Constants.Version,
        ViaDevice: BridgeDeviceId);

    [SuppressMessage(
        "Minor Code Smell",
        "S3267:Loops should be simplified with \"LINQ\" expressions",
        Justification = "Reads worse and is less performant")]
    private async Task PublishDiscoveryMessagesAsync<TState>(
        StateUpdateEventArgs<TState> stateUpdate,
        HashSet<SteamId64> publishedConfigSet,
        Func<Device, MqttDiscoveryMessages> discoveryMessages,
        CancellationToken cancellationToken)
        where TState : class
    {
        var device = devices.GetOrAdd(stateUpdate.SteamId, CreateDevice);

        if (!publishedConfigSet.Add(stateUpdate.SteamId))
        {
            return;
        }

        foreach (var discoveryMessage in discoveryMessages(device))
        {
            await mqttClient.PublishAsync(discoveryMessage, cancellationToken);
        }
    }

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
