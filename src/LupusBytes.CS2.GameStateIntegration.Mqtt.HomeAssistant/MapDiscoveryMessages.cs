namespace LupusBytes.CS2.GameStateIntegration.Mqtt.HomeAssistant;

public class MapDiscoveryMessages(Device device) : MqttDiscoveryMessages
{
    private readonly string stateTopic = $"{MqttConstants.BaseTopic}/{device.Id}/map";

    private readonly Availability[] availability =
    [
        new($"{MqttConstants.BaseTopic}/{device.Id}/map/status"),
        new($"{MqttConstants.BaseTopic}/{device.Id}/status"),
        new(MqttConstants.SystemAvailabilityTopic),
    ];

    protected override IEnumerable<MqttMessage> DiscoveryMessages
    {
        get
        {
            yield return NameDiscoveryMessage;
            yield return ModeDiscoveryMessage;
            yield return MapPhaseDiscoveryMessage;
            yield return RoundNumberDiscoveryMessage;
            yield return TScoreDiscoveryMessage;
            yield return CTScoreDiscoveryMessage;
        }
    }

    private MqttMessage NameDiscoveryMessage => CreateMqttMessage(new SensorConfig(
        Name: "Map",
        UniqueId: $"{device.Id}_map_name",
        StateTopic: stateTopic,
        ValueTemplate: ValueTemplate.JsonPropertyValue("name"),
        Icon: "mdi:map",
        device,
        Availability: availability));

    private MqttMessage ModeDiscoveryMessage => CreateMqttMessage(new SensorConfig(
        Name: "Mode",
        UniqueId: $"{device.Id}_map_mode",
        StateTopic: stateTopic,
        ValueTemplate: ValueTemplate.JsonPropertyValue("mode"),
        Icon: "mdi:target-account",
        device,
        Availability: availability));

    private MqttMessage MapPhaseDiscoveryMessage => CreateMqttMessage(new SensorConfig(
        Name: "Map phase",
        UniqueId: $"{device.Id}_map_phase",
        StateTopic: stateTopic,
        ValueTemplate: ValueTemplate.JsonPropertyValue("phase"),
        Icon: "mdi:clock-outline",
        device,
        Availability: availability));

    private MqttMessage RoundNumberDiscoveryMessage => CreateMqttMessage(new SensorConfig(
        Name: "Round",
        UniqueId: $"{device.Id}_map_round",
        StateTopic: stateTopic,
        ValueTemplate: ValueTemplate.JsonPropertyValue("round"),
        Icon: "mdi:counter",
        device,
        Availability: availability,
        UnitOfMeasurement: "rounds"));

    private MqttMessage TScoreDiscoveryMessage => CreateMqttMessage(new SensorConfig(
        Name: "Team T Score",
        UniqueId: $"{device.Id}_map_t_score",
        StateTopic: stateTopic,
        ValueTemplate: ValueTemplate.NestedJsonPropertyValue("team_t", "score"),
        Icon: "mdi:account-group",
        device,
        Availability: availability,
        UnitOfMeasurement: "rounds"));

    private MqttMessage CTScoreDiscoveryMessage => CreateMqttMessage(new SensorConfig(
        Name: "Team CT Score",
        UniqueId: $"{device.Id}_map_ct_score",
        StateTopic: stateTopic,
        ValueTemplate: ValueTemplate.NestedJsonPropertyValue("team_ct", "score"),
        Icon: "mdi:account-group-outline",
        device,
        Availability: availability,
        UnitOfMeasurement: "rounds"));
}