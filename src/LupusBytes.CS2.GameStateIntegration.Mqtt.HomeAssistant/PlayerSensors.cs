namespace LupusBytes.CS2.GameStateIntegration.Mqtt.HomeAssistant;

public class PlayerSensors(Device device) : IDeviceSensors
{
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
            StateTopic: $"cs2mqtt/{device.Id}/player",
            ValueTemplate: "{{ value_json.steamid }}",
            Icon: "mdi:identifier",
            device,
            Availability: [new($"cs2mqtt/{device.Id}/player/status"), new("cs2mqtt/status")]);

    private DiscoveryPayload NameDiscoveryPayload =>
        new(
            Name: "Player name",
            UniqueId: $"{device.Id}_player_name",
            StateTopic: $"cs2mqtt/{device.Id}/player",
            ValueTemplate: "{{ value_json.name }}",
            Icon: "mdi:account",
            device,
            Availability: [new($"cs2mqtt/{device.Id}/player/status"), new("cs2mqtt/status")]);

    private DiscoveryPayload TeamDiscoveryPayload =>
        new(
            Name: "Current team",
            UniqueId: $"{device.Id}_player_team",
            StateTopic: $"cs2mqtt/{device.Id}/player",
            ValueTemplate: "{{ value_json.team }}",
            Icon: "mdi:account-multiple-outline",
            device,
            Availability: [new($"cs2mqtt/{device.Id}/player/status"), new("cs2mqtt/status")]);

    private DiscoveryPayload ActivityDiscoveryPayload =>
        new(
            Name: "Current activity",
            UniqueId: $"{device.Id}_player_activity",
            StateTopic: $"cs2mqtt/{device.Id}/player",
            ValueTemplate: "{{ value_json.activity }}",
            Icon: "mdi:play",
            device,
            Availability: [new($"cs2mqtt/{device.Id}/player/status"), new("cs2mqtt/status")]);
}