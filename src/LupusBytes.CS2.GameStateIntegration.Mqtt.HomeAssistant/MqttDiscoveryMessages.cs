using System.Collections;
using System.Text.Json;

namespace LupusBytes.CS2.GameStateIntegration.Mqtt.HomeAssistant;

public abstract class MqttDiscoveryMessages : IEnumerable<MqttMessage>
{
    // Explicit interface implementation for non-generic enumerator
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public IEnumerator<MqttMessage> GetEnumerator() => DiscoveryMessages.GetEnumerator();

    protected abstract IEnumerable<MqttMessage> DiscoveryMessages { get; }

    protected static MqttMessage CreateMqttMessage(SensorConfig config) => new(
        $"homeassistant/sensor/{config.UniqueId}/config",
        JsonSerializer.Serialize(config),
        RetainFlag: true);

    protected static MqttMessage CreateMqttMessage(BinarySensorConfig config) => new(
        $"homeassistant/binary_sensor/{config.UniqueId}/config",
        JsonSerializer.Serialize(config),
        RetainFlag: true);
}