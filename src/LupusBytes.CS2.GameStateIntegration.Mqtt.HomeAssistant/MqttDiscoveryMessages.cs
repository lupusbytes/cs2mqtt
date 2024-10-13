using System.Collections;
using System.Text.Json;

namespace LupusBytes.CS2.GameStateIntegration.Mqtt.HomeAssistant;

public abstract class MqttDiscoveryMessages : IEnumerable<MqttMessage>
{
    // Explicit interface implementation for non-generic enumerator
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public IEnumerator<MqttMessage> GetEnumerator() => DiscoveryMessages.GetEnumerator();

    protected abstract IEnumerable<MqttMessage> DiscoveryMessages { get; }

    protected static MqttMessage CreateMqttMessage(SensorConfig sensorConfig) => new(
        $"homeassistant/sensor/{sensorConfig.UniqueId}/config",
        JsonSerializer.Serialize(sensorConfig),
        RetainFlag: true);
}