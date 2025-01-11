namespace LupusBytes.CS2.GameStateIntegration.Mqtt.HomeAssistant;

public class PlayerDiscoveryMessages(Device device) : MqttDiscoveryMessages
{
    private readonly string stateTopic = $"{MqttConstants.BaseTopic}/{device.Id}/player";

    private readonly Availability[] availability =
    [
        new($"{MqttConstants.BaseTopic}/{device.Id}/player/status"),
        new($"{MqttConstants.BaseTopic}/{device.Id}/status"),
        new(MqttConstants.SystemAvailabilityTopic),
    ];

    protected override IEnumerable<MqttMessage> DiscoveryMessages
    {
        get
        {
            yield return SteamIdDiscoveryMessage;
            yield return NameDiscoveryMessage;
            yield return TeamDiscoveryMessage;
            yield return ActivityDiscoveryMessage;
        }
    }

    private MqttMessage SteamIdDiscoveryMessage => CreateMqttMessage(new SensorConfig(
        Name: "SteamID64",
        UniqueId: $"{device.Id}_player_steamid",
        StateTopic: stateTopic,
        ValueTemplate: ValueTemplate.JsonPropertyValue("steamid"),
        Icon: "mdi:identifier",
        device,
        Availability: availability));

    private MqttMessage NameDiscoveryMessage => CreateMqttMessage(new SensorConfig(
        Name: "Player name",
        UniqueId: $"{device.Id}_player_name",
        StateTopic: stateTopic,
        ValueTemplate: ValueTemplate.JsonPropertyValue("name"),
        Icon: "mdi:account",
        device,
        Availability: availability));

    private MqttMessage TeamDiscoveryMessage => CreateMqttMessage(new SensorConfig(
        Name: "Current team",
        UniqueId: $"{device.Id}_player_team",
        StateTopic: stateTopic,
        ValueTemplate: ValueTemplate.JsonPropertyValue("team"),
        Icon: "mdi:account-multiple-outline",
        device,
        Availability: availability));

    private MqttMessage ActivityDiscoveryMessage => CreateMqttMessage(new SensorConfig(
        Name: "Current activity",
        UniqueId: $"{device.Id}_player_activity",
        StateTopic: stateTopic,
        ValueTemplate: ValueTemplate.JsonPropertyValue("activity"),
        Icon: "mdi:play",
        device,
        Availability: availability));
}