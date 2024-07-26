namespace LupusBytes.CS2.GameStateIntegration.Mqtt.HomeAssistant;

public class PlayerStateSensors(Device device) : IDeviceSensors
{
    private readonly string stateTopic = $"{MqttConstants.BaseTopic}/{device.Id}/player-state";

    private readonly Availability[] availability =
    [
        new($"{MqttConstants.BaseTopic}/{device.Id}/player-state/status"),
        new(MqttConstants.SystemAvailabilityTopic),
    ];

    public IEnumerable<DiscoveryPayload> DiscoveryPayloads
    {
        get
        {
            yield return HealthDiscoveryPayload;
            yield return ArmorDiscoveryPayload;
            yield return HelmetDiscoveryPayload;
            yield return FlashedDiscoveryPayload;
            yield return SmokedDiscoveryPayload;
            yield return BurningDiscoveryPayload;
            yield return MoneyDiscoveryPayload;
            yield return RoundKillsDiscoveryPayload;
            yield return RoundHeadshotsDiscoveryPayload;
            yield return EquipmentValueDiscoveryPayload;
        }
    }

    private DiscoveryPayload HealthDiscoveryPayload =>
        new(
            Name: "Health",
            UniqueId: $"{device.Id}_player-state_health",
            StateTopic: stateTopic,
            ValueTemplate: "{{ value_json.health }}",
            Icon: "mdi:medical-bag",
            device,
            Availability: availability);

    private DiscoveryPayload ArmorDiscoveryPayload =>
        new(
            Name: "Armor",
            UniqueId: $"{device.Id}_player-state_armor",
            StateTopic: stateTopic,
            ValueTemplate: "{{ value_json.armor }}",
            Icon: "mdi:shield",
            device,
            Availability: availability);

    private DiscoveryPayload HelmetDiscoveryPayload =>
        new(
            Name: "Helmet",
            UniqueId: $"{device.Id}_player-state_helmet",
            StateTopic: stateTopic,
            ValueTemplate: "{{ value_json.helmet }}",
            Icon: "mdi:account-hard-hat",
            device,
            Availability: availability);

    private DiscoveryPayload FlashedDiscoveryPayload =>
        new(
            Name: "Flashed",
            UniqueId: $"{device.Id}_player-state_flashed",
            StateTopic: stateTopic,
            ValueTemplate: "{{ value_json.flashed }}",
            Icon: "mdi:flash-alert",
            device,
            Availability: availability);

    private DiscoveryPayload SmokedDiscoveryPayload =>
        new(
            Name: "Smoked",
            UniqueId: $"{device.Id}_player-state_smoke",
            StateTopic: stateTopic,
            ValueTemplate: "{{ value_json.smoked }}",
            Icon: "mdi:weather-fog",
            device,
            Availability: availability);

    private DiscoveryPayload BurningDiscoveryPayload =>
        new(
            Name: "Burning",
            UniqueId: $"{device.Id}_player-state_burning",
            StateTopic: stateTopic,
            ValueTemplate: "{{ value_json.burning }}",
            Icon: "mdi:fire-alert",
            device,
            Availability: availability);

    private DiscoveryPayload MoneyDiscoveryPayload =>
        new(
            Name: "Money",
            UniqueId: $"{device.Id}_player-state_money",
            StateTopic: stateTopic,
            ValueTemplate: "{{ value_json.money }}",
            Icon: "mdi:currency-usd",
            device,
            Availability: availability);

    private DiscoveryPayload RoundKillsDiscoveryPayload =>
        new(
            Name: "Round kills",
            UniqueId: $"{device.Id}_player-state_round_kills",
            StateTopic: stateTopic,
            ValueTemplate: "{{ value_json.round_kills }}",
            Icon: "mdi:account-alert",
            device,
            Availability: availability);

    private DiscoveryPayload RoundHeadshotsDiscoveryPayload =>
        new(
            Name: "Round headshots",
            UniqueId: $"{device.Id}_player-state_round_killhs",
            StateTopic: stateTopic,
            ValueTemplate: "{{ value_json.round_killhs }}",
            Icon: "mdi:head-alert",
            device,
            Availability: availability);

    private DiscoveryPayload EquipmentValueDiscoveryPayload =>
        new(
            Name: "Equipment value",
            UniqueId: $"{device.Id}_player-state_equip_value",
            StateTopic: stateTopic,
            ValueTemplate: "{{ value_json.equip_value }}",
            Icon: "mdi:currency-usd",
            device,
            Availability: availability);
}