namespace LupusBytes.CS2.GameStateIntegration.Mqtt.HomeAssistant;

public class PlayerSensors(Device device) : IDeviceSensors
{
    private readonly string stateTopic = $"{MqttConstants.BaseTopic}/{device.Id}/player";

    private readonly Availability[] availability =
    [
        new($"{MqttConstants.BaseTopic}/{device.Id}/player/status"),
        new(MqttConstants.SystemAvailabilityTopic),
    ];

    public IEnumerable<DiscoveryPayload> DiscoveryPayloads
    {
        get
        {
            yield return SteamIdDiscoveryPayload;
            yield return NameDiscoveryPayload;
            yield return TeamDiscoveryPayload;
            yield return ActivityDiscoveryPayload;
        }
    }

    private DiscoveryPayload SteamIdDiscoveryPayload =>
        new(
            Name: "SteamID64",
            UniqueId: $"{device.Id}_player_steamid",
            StateTopic: stateTopic,
            ValueTemplate: "{{ value_json.steamid }}",
            Icon: "mdi:identifier",
            device,
            Availability: availability);

    private DiscoveryPayload NameDiscoveryPayload =>
        new(
            Name: "Player name",
            UniqueId: $"{device.Id}_player_name",
            StateTopic: stateTopic,
            ValueTemplate: "{{ value_json.name }}",
            Icon: "mdi:account",
            device,
            Availability: availability);

    private DiscoveryPayload TeamDiscoveryPayload =>
        new(
            Name: "Current team",
            UniqueId: $"{device.Id}_player_team",
            StateTopic: stateTopic,
            ValueTemplate: "{{ value_json.team }}",
            Icon: "mdi:account-multiple-outline",
            device,
            Availability: availability);

    private DiscoveryPayload ActivityDiscoveryPayload =>
        new(
            Name: "Current activity",
            UniqueId: $"{device.Id}_player_activity",
            StateTopic: stateTopic,
            ValueTemplate: "{{ value_json.activity }}",
            Icon: "mdi:play",
            device,
            Availability: availability);
}