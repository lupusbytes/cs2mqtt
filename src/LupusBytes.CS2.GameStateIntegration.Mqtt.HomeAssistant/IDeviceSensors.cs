namespace LupusBytes.CS2.GameStateIntegration.Mqtt.HomeAssistant;

public interface IDeviceSensors
{
    IEnumerable<DiscoveryPayload> DiscoveryPayloads { get; }
}