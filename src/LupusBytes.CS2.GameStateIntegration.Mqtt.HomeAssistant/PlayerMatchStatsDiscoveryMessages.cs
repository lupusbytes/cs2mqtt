namespace LupusBytes.CS2.GameStateIntegration.Mqtt.HomeAssistant;

public class PlayerMatchStatsDiscoveryMessages(Device device) : MqttDiscoveryMessages
{
    private readonly string stateTopic = $"{MqttConstants.BaseTopic}/{device.Id}/player-match-stats";

    private readonly Availability[] availability =
    [
        new($"{MqttConstants.BaseTopic}/{device.Id}/player-match-stats/status"),
        new($"{MqttConstants.BaseTopic}/{device.Id}/status"),
        new(MqttConstants.SystemAvailabilityTopic),
    ];

    protected override IEnumerable<MqttMessage> DiscoveryMessages
    {
        get
        {
            yield return KillsDiscoveryMessage;
            yield return AssistsDiscoveryMessage;
            yield return DeathsDiscoveryMessage;
            yield return MvpsDiscoveryMessage;
            yield return ScoreDiscoveryMessage;
        }
    }

    private MqttMessage KillsDiscoveryMessage => CreateMqttMessage(new SensorConfig(
        Name: "Kills",
        UniqueId: $"{device.Id}_player-match-stats_kills",
        StateTopic: stateTopic,
        ValueTemplate: ValueTemplate.JsonPropertyValue("kills"),
        Icon: "mdi:account-alert",
        device,
        Availability: availability,
        StateClass: StateClass.Measurement));

    private MqttMessage AssistsDiscoveryMessage => CreateMqttMessage(new SensorConfig(
        Name: "Assists",
        UniqueId: $"{device.Id}_player-match-stats_assists",
        StateTopic: stateTopic,
        ValueTemplate: ValueTemplate.JsonPropertyValue("assists"),
        Icon: "mdi:account-plus",
        device,
        Availability: availability,
        StateClass: StateClass.Measurement));

    private MqttMessage DeathsDiscoveryMessage => CreateMqttMessage(new SensorConfig(
        Name: "Deaths",
        UniqueId: $"{device.Id}_player-match-stats_deaths",
        StateTopic: stateTopic,
        ValueTemplate: ValueTemplate.JsonPropertyValue("deaths"),
        Icon: "mdi:skull-crossbones",
        device,
        Availability: availability,
        StateClass: StateClass.Measurement));

    private MqttMessage MvpsDiscoveryMessage => CreateMqttMessage(new SensorConfig(
        Name: "MVPs",
        UniqueId: $"{device.Id}_player-match-stats_mvps",
        StateTopic: stateTopic,
        ValueTemplate: ValueTemplate.JsonPropertyValue("mvps"),
        Icon: "mdi:medal",
        device,
        Availability: availability,
        StateClass: StateClass.Measurement));

    private MqttMessage ScoreDiscoveryMessage => CreateMqttMessage(new SensorConfig(
        Name: "Score",
        UniqueId: $"{device.Id}_player-match-stats_score",
        StateTopic: stateTopic,
        ValueTemplate: ValueTemplate.JsonPropertyValue("score"),
        Icon: "mdi:trending-up",
        device,
        Availability: availability,
        StateClass: StateClass.Measurement));
}