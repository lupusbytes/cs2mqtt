namespace LupusBytes.CS2.GameStateIntegration.Mqtt.HomeAssistant;

public class RoundSensors(Device device) : IDeviceSensors
{
    public IEnumerable<DiscoveryPayload> DiscoveryPayloads
    {
        get
        {
            yield return RoundPhaseDiscoveryPayload;
            yield return RoundWinningTeamDiscoveryPayload;
            yield return BombDiscoveryPayload;
        }
    }

    private DiscoveryPayload RoundPhaseDiscoveryPayload =>
        new(
            Name: "Round phase",
            UniqueId: $"{device.Id}_round_phase",
            StateTopic: $"cs2mqtt/{device.Id}/round",
            ValueTemplate: "{{ value_json.phase }}",
            Icon: "mdi:clock-outline",
            device,
            new Availability($"cs2mqtt/{device.Id}/round/status"));

    private DiscoveryPayload RoundWinningTeamDiscoveryPayload =>
        new(
            Name: "Round winning team",
            UniqueId: $"{device.Id}_round_win_team",
            StateTopic: $"cs2mqtt/{device.Id}/round",
            ValueTemplate: "{{ value_json.win_team }}",
            Icon: "mdi:trophy",
            device,
            new Availability($"cs2mqtt/{device.Id}/round/status"));

    private DiscoveryPayload BombDiscoveryPayload =>
        new(
            Name: "Bomb",
            UniqueId: $"{device.Id}_round_bomb",
            StateTopic: $"cs2mqtt/{device.Id}/round",
            ValueTemplate: "{{ value_json.bomb }}",
            Icon: "mdi:bomb",
            device,
            new Availability($"cs2mqtt/{device.Id}/round/status"));
}