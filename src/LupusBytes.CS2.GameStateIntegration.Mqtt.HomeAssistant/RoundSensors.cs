namespace LupusBytes.CS2.GameStateIntegration.Mqtt.HomeAssistant;

public class RoundSensors(Device device) : IDeviceSensors
{
    private readonly string stateTopic = $"{MqttConstants.BaseTopic}/{device.Id}/round";

    private readonly Availability[] availability =
    [
        new($"{MqttConstants.BaseTopic}/{device.Id}/round/status"),
        new(MqttConstants.SystemAvailabilityTopic),
    ];

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
            StateTopic: stateTopic,
            ValueTemplate: "{{ value_json.phase }}",
            Icon: "mdi:clock-outline",
            device,
            Availability: availability);

    private DiscoveryPayload RoundWinningTeamDiscoveryPayload =>
        new(
            Name: "Round winning team",
            UniqueId: $"{device.Id}_round_win_team",
            StateTopic: stateTopic,
            ValueTemplate: "{{ value_json.win_team }}",
            Icon: "mdi:trophy",
            device,
            Availability: availability);

    private DiscoveryPayload BombDiscoveryPayload =>
        new(
            Name: "Bomb",
            UniqueId: $"{device.Id}_round_bomb",
            StateTopic: stateTopic,
            ValueTemplate: "{{ value_json.bomb }}",
            Icon: "mdi:bomb",
            device,
            Availability: availability);
}