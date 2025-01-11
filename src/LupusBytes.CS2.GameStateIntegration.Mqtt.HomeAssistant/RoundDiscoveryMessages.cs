namespace LupusBytes.CS2.GameStateIntegration.Mqtt.HomeAssistant;

public class RoundDiscoveryMessages(Device device) : MqttDiscoveryMessages
{
    private readonly string stateTopic = $"{MqttConstants.BaseTopic}/{device.Id}/round";

    private readonly Availability[] availability =
    [
        new($"{MqttConstants.BaseTopic}/{device.Id}/round/status"),
        new($"{MqttConstants.BaseTopic}/{device.Id}/status"),
        new(MqttConstants.SystemAvailabilityTopic),
    ];

    protected override IEnumerable<MqttMessage> DiscoveryMessages
    {
        get
        {
            yield return RoundPhaseDiscoveryMessage;
            yield return RoundWinningTeamDiscoveryMessage;
            yield return BombDiscoveryMessage;
        }
    }

    private MqttMessage RoundPhaseDiscoveryMessage => CreateMqttMessage(new SensorConfig(
        Name: "Round phase",
        UniqueId: $"{device.Id}_round_phase",
        StateTopic: stateTopic,
        ValueTemplate: ValueTemplate.JsonPropertyValue("phase"),
        Icon: "mdi:clock-outline",
        device,
        Availability: availability));

    private MqttMessage RoundWinningTeamDiscoveryMessage => CreateMqttMessage(new SensorConfig(
        Name: "Round winning team",
        UniqueId: $"{device.Id}_round_win_team",
        StateTopic: stateTopic,
        ValueTemplate: ValueTemplate.JsonPropertyValue("win_team"),
        Icon: "mdi:trophy",
        device,
        Availability: availability));

    private MqttMessage BombDiscoveryMessage => CreateMqttMessage(new SensorConfig(
        Name: "Bomb",
        UniqueId: $"{device.Id}_round_bomb",
        StateTopic: stateTopic,
        ValueTemplate: ValueTemplate.JsonPropertyValueWithFallback("bomb", "Inactive"),
        Icon: "mdi:bomb",
        device,
        Availability: availability));
}