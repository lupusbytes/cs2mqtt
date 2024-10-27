using System.Text.Json.Serialization;

namespace LupusBytes.CS2.GameStateIntegration.Mqtt.HomeAssistant;

public record BinarySensorConfig(
    [property: JsonPropertyName("name")] string Name,
    [property: JsonPropertyName("unique_id")] string UniqueId,
    [property: JsonPropertyName("state_topic")] string StateTopic,
    [property: JsonPropertyName("device")] Device Device,
    [property: JsonPropertyName("device_class")] string DeviceClass,
    [property: JsonPropertyName("entity_category")] string EntityCategory,
    [property: JsonPropertyName("payload_off")] string PayloadOff = "offline",
    [property: JsonPropertyName("payload_on")] string PayloadOn = "online",
    [property: JsonPropertyName("value_template")] string ValueTemplate = "{{ value }}");