namespace LupusBytes.CS2.GameStateIntegration.Mqtt.HomeAssistant;

public class BridgeDiscoveryMessages(Device device) : MqttDiscoveryMessages
{
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
        StateTopic: MqttConstants.SystemAvailabilityTopic,
        Device: device,
        DeviceClass: "connectivity",
        EntityCategory: "diagnostic"));
}