namespace LupusBytes.CS2.GameStateIntegration.Mqtt.HomeAssistant;

public class PlayerStateDiscoveryMessages(Device device) : MqttDiscoveryMessages
{
    private readonly string stateTopic = $"{MqttConstants.BaseTopic}/{device.Id}/player-state";

    private readonly Availability[] availability =
    [
        new($"{MqttConstants.BaseTopic}/{device.Id}/player-state/status"),
        new($"{MqttConstants.BaseTopic}/{device.Id}/status"),
        new(MqttConstants.SystemAvailabilityTopic),
    ];

    protected override IEnumerable<MqttMessage> DiscoveryMessages
    {
        get
        {
            yield return HealthDiscoveryMessage;
            yield return ArmorDiscoveryMessage;
            yield return HelmetDiscoveryMessage;
            yield return FlashedDiscoveryMessage;
            yield return SmokedDiscoveryMessage;
            yield return BurningDiscoveryMessage;
            yield return MoneyDiscoveryMessage;
            yield return RoundKillsDiscoveryMessage;
            yield return RoundHeadshotsDiscoveryMessage;
            yield return EquipmentValueDiscoveryMessage;
        }
    }

    private MqttMessage HealthDiscoveryMessage => CreateMqttMessage(new SensorConfig(
        Name: "Health",
        UniqueId: $"{device.Id}_player-state_health",
        StateTopic: stateTopic,
        ValueTemplate: ValueTemplate.JsonPropertyValue("health"),
        Icon: "mdi:medical-bag",
        device,
        Availability: availability,
        StateClass: StateClass.Measurement));

    private MqttMessage ArmorDiscoveryMessage => CreateMqttMessage(new SensorConfig(
        Name: "Armor",
        UniqueId: $"{device.Id}_player-state_armor",
        StateTopic: stateTopic,
        ValueTemplate: ValueTemplate.JsonPropertyValue("armor"),
        Icon: "mdi:shield",
        device,
        Availability: availability,
        StateClass: StateClass.Measurement));

    private MqttMessage HelmetDiscoveryMessage => CreateMqttMessage(new SensorConfig(
        Name: "Helmet",
        UniqueId: $"{device.Id}_player-state_helmet",
        StateTopic: stateTopic,
        ValueTemplate: ValueTemplate.JsonPropertyValue("helmet"),
        Icon: "mdi:account-hard-hat",
        device,
        Availability: availability));

    private MqttMessage FlashedDiscoveryMessage => CreateMqttMessage(new SensorConfig(
        Name: "Flashed",
        UniqueId: $"{device.Id}_player-state_flashed",
        StateTopic: stateTopic,
        ValueTemplate: ValueTemplate.JsonPropertyValue("flashed"),
        Icon: "mdi:flash-alert",
        device,
        Availability: availability));

    private MqttMessage SmokedDiscoveryMessage => CreateMqttMessage(new SensorConfig(
        Name: "Smoked",
        UniqueId: $"{device.Id}_player-state_smoke",
        StateTopic: stateTopic,
        ValueTemplate: ValueTemplate.JsonPropertyValueWithByteToPercentConversion("smoked"),
        Icon: "mdi:weather-fog",
        device,
        Availability: availability,
        UnitOfMeasurement: "%",
        StateClass: StateClass.Measurement));

    private MqttMessage BurningDiscoveryMessage => CreateMqttMessage(new SensorConfig(
        Name: "Burning",
        UniqueId: $"{device.Id}_player-state_burning",
        StateTopic: stateTopic,
        ValueTemplate: ValueTemplate.JsonPropertyValueWithByteToPercentConversion("burning"),
        Icon: "mdi:fire-alert",
        device,
        Availability: availability,
        UnitOfMeasurement: "%",
        StateClass: StateClass.Measurement));

    private MqttMessage MoneyDiscoveryMessage => CreateMqttMessage(new SensorConfig(
        Name: "Money",
        UniqueId: $"{device.Id}_player-state_money",
        StateTopic: stateTopic,
        ValueTemplate: ValueTemplate.JsonPropertyValue("money"),
        Icon: "mdi:currency-usd",
        device,
        Availability: availability,
        UnitOfMeasurement: "$",
        StateClass: StateClass.Measurement));

    private MqttMessage RoundKillsDiscoveryMessage => CreateMqttMessage(new SensorConfig(
        Name: "Round kills",
        UniqueId: $"{device.Id}_player-state_round_kills",
        StateTopic: stateTopic,
        ValueTemplate: ValueTemplate.JsonPropertyValue("round_kills"),
        Icon: "mdi:account-alert",
        device,
        Availability: availability,
        StateClass: StateClass.Measurement));

    private MqttMessage RoundHeadshotsDiscoveryMessage => CreateMqttMessage(new SensorConfig(
        Name: "Round headshots",
        UniqueId: $"{device.Id}_player-state_round_killhs",
        StateTopic: stateTopic,
        ValueTemplate: ValueTemplate.JsonPropertyValue("round_killhs"),
        Icon: "mdi:head-alert",
        device,
        Availability: availability,
        StateClass: StateClass.Measurement));

    private MqttMessage EquipmentValueDiscoveryMessage => CreateMqttMessage(new SensorConfig(
        Name: "Equipment value",
        UniqueId: $"{device.Id}_player-state_equip_value",
        StateTopic: stateTopic,
        ValueTemplate: ValueTemplate.JsonPropertyValue("equip_value"),
        Icon: "mdi:currency-usd",
        device,
        Availability: availability,
        UnitOfMeasurement: "$",
        StateClass: StateClass.Measurement));
}