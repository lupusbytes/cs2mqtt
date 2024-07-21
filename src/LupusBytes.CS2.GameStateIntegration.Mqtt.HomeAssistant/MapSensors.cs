namespace LupusBytes.CS2.GameStateIntegration.Mqtt.HomeAssistant;

public class MapSensors(Device device) : IDeviceSensors
{
    public IEnumerable<DiscoveryPayload> DiscoveryPayloads
    {
        get
        {
            yield return NameDiscoveryPayload;
            yield return ModeDiscoveryPayload;
            yield return MapPhaseDiscoveryPayload;
            yield return RoundNumberDiscoveryPayload;
            yield return TScoreDiscoveryPayload;
            yield return CTScoreDiscoveryPayload;
        }
    }

    private DiscoveryPayload NameDiscoveryPayload =>
        new(
            Name: "Map",
            UniqueId: $"{device.Id}_map_name",
            StateTopic: $"cs2mqtt/{device.Id}/map",
            ValueTemplate: "{{ value_json.name }}",
            Icon: "mdi:map",
            device,
            Availability: [new($"cs2mqtt/{device.Id}/map/status"), new("cs2mqtt/status")]);

    private DiscoveryPayload ModeDiscoveryPayload =>
        new(
            Name: "Mode",
            UniqueId: $"{device.Id}_map_mode",
            StateTopic: $"cs2mqtt/{device.Id}/map",
            ValueTemplate: "{{ value_json.mode }}",
            Icon: "mdi:target-account",
            device,
            Availability: [new($"cs2mqtt/{device.Id}/map/status"), new("cs2mqtt/status")]);

    private DiscoveryPayload MapPhaseDiscoveryPayload =>
        new(
            Name: "Map phase",
            UniqueId: $"{device.Id}_map_phase",
            StateTopic: $"cs2mqtt/{device.Id}/map",
            ValueTemplate: "{{ value_json.phase }}",
            Icon: "mdi:clock-outline",
            device,
            Availability: [new($"cs2mqtt/{device.Id}/map/status"), new("cs2mqtt/status")]);

    private DiscoveryPayload RoundNumberDiscoveryPayload =>
        new(
            Name: "Round",
            UniqueId: $"{device.Id}_map_round",
            StateTopic: $"cs2mqtt/{device.Id}/map",
            ValueTemplate: "{{ value_json.round }}",
            Icon: "mdi:counter",
            device,
            Availability: [new($"cs2mqtt/{device.Id}/map/status"), new("cs2mqtt/status")]);

    private DiscoveryPayload TScoreDiscoveryPayload =>
        new(
            Name: "Team T Score",
            UniqueId: $"{device.Id}_map_t_score",
            StateTopic: $"cs2mqtt/{device.Id}/map",
            ValueTemplate: "{{ value_json[\"team_t\"].score }}",
            Icon: "mdi:account-group",
            device,
            Availability: [new($"cs2mqtt/{device.Id}/map/status"), new("cs2mqtt/status")]);

    private DiscoveryPayload CTScoreDiscoveryPayload =>
        new(
            Name: "Team CT Score",
            UniqueId: $"{device.Id}_map_ct_score",
            StateTopic: $"cs2mqtt/{device.Id}/map",
            ValueTemplate: "{{ value_json[\"team_ct\"].score }}",
            Icon: "mdi:account-group-outline",
            device,
            Availability: [new($"cs2mqtt/{device.Id}/map/status"), new("cs2mqtt/status")]);
}