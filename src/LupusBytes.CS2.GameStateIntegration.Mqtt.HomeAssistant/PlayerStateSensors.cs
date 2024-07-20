namespace LupusBytes.CS2.GameStateIntegration.Mqtt.HomeAssistant;

public class PlayerStateSensors(Device device) : IDeviceSensors
{
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
            StateTopic: $"cs2mqtt/{device.Id}/player-state",
            ValueTemplate: "{{ value_json.health }}",
            Icon: "mdi:medical-bag",
            device,
            new Availability($"cs2mqtt/{device.Id}/player-state/status"));

    private DiscoveryPayload ArmorDiscoveryPayload =>
        new(
            Name: "Armor",
            UniqueId: $"{device.Id}_player-state_armor",
            StateTopic: $"cs2mqtt/{device.Id}/player-state",
            ValueTemplate: "{{ value_json.armor }}",
            Icon: "mdi:shield",
            device,
            new Availability($"cs2mqtt/{device.Id}/player-state/status"));

    private DiscoveryPayload HelmetDiscoveryPayload =>
        new(
            Name: "Helmet",
            UniqueId: $"{device.Id}_player-state_helmet",
            StateTopic: $"cs2mqtt/{device.Id}/player-state",
            ValueTemplate: "{{ value_json.helmet }}",
            Icon: "mdi:account-hard-hat",
            device,
            new Availability($"cs2mqtt/{device.Id}/player-state/status"));

    private DiscoveryPayload FlashedDiscoveryPayload =>
        new(
            Name: "Flashed",
            UniqueId: $"{device.Id}_player-state_flashed",
            StateTopic: $"cs2mqtt/{device.Id}/player-state",
            ValueTemplate: "{{ value_json.flashed }}",
            Icon: "mdi:flash-alert",
            device,
            new Availability($"cs2mqtt/{device.Id}/player-state/status"));

    private DiscoveryPayload SmokedDiscoveryPayload =>
        new(
            Name: "Smoked",
            UniqueId: $"{device.Id}_player-state_smoke",
            StateTopic: $"cs2mqtt/{device.Id}/player-state",
            ValueTemplate: "{{ value_json.smoked }}",
            Icon: "mdi:weather-fog",
            device,
            new Availability($"cs2mqtt/{device.Id}/player-state/status"));

    private DiscoveryPayload BurningDiscoveryPayload =>
        new(
            Name: "Burning",
            UniqueId: $"{device.Id}_player-state_burning",
            StateTopic: $"cs2mqtt/{device.Id}/player-state",
            ValueTemplate: "{{ value_json.burning }}",
            Icon: "mdi:fire-alert",
            device,
            new Availability($"cs2mqtt/{device.Id}/player-state/status"));

    private DiscoveryPayload MoneyDiscoveryPayload =>
        new(
            Name: "Money",
            UniqueId: $"{device.Id}_player-state_money",
            StateTopic: $"cs2mqtt/{device.Id}/player-state",
            ValueTemplate: "{{ value_json.money }}",
            Icon: "mdi:currency-usd",
            device,
            new Availability($"cs2mqtt/{device.Id}/player-state/status"));

    private DiscoveryPayload RoundKillsDiscoveryPayload =>
        new(
            Name: "Round kills",
            UniqueId: $"{device.Id}_player-state_round_kills",
            StateTopic: $"cs2mqtt/{device.Id}/player-state",
            ValueTemplate: "{{ value_json.round_kills }}",
            Icon: "mdi:account-alert",
            device,
            new Availability($"cs2mqtt/{device.Id}/player-state/status"));

    private DiscoveryPayload RoundHeadshotsDiscoveryPayload =>
        new(
            Name: "Round headshots",
            UniqueId: $"{device.Id}_player-state_round_killhs",
            StateTopic: $"cs2mqtt/{device.Id}/player-state",
            ValueTemplate: "{{ value_json.round_killhs }}",
            Icon: "mdi:head-alert",
            device,
            new Availability($"cs2mqtt/{device.Id}/player-state/status"));

    private DiscoveryPayload EquipmentValueDiscoveryPayload =>
        new(
            Name: "Equipment value",
            UniqueId: $"{device.Id}_player-state_equip_value",
            StateTopic: $"cs2mqtt/{device.Id}/player-state",
            ValueTemplate: "{{ value_json.equip_value }}",
            Icon: "mdi:currency-usd",
            device,
            new Availability($"cs2mqtt/{device.Id}/player-state/status"));
}