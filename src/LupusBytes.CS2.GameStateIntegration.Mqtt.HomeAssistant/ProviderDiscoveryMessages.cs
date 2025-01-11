namespace LupusBytes.CS2.GameStateIntegration.Mqtt.HomeAssistant;

public class ProviderDiscoveryMessages(Device device) : MqttDiscoveryMessages
{
    private readonly string stateTopic = $"{MqttConstants.BaseTopic}/{device.Id}/status";
    private readonly Availability[] availability =
    [
        new(MqttConstants.SystemAvailabilityTopic),
    ];

    protected override IEnumerable<MqttMessage> DiscoveryMessages
    {
        get
        {
            yield return ConnectionStateDiscoveryMessage;
        }
    }

    private MqttMessage ConnectionStateDiscoveryMessage => CreateMqttMessage(new BinarySensorConfig(
        Name: "Connection state",
        UniqueId: $"{device.Id}_connection_state",
        StateTopic: stateTopic,
        Device: device,
        DeviceClass: "connectivity",
        EntityCategory: "diagnostic",
        Availability: availability));
}