using System.Text.Json.Serialization;

namespace LupusBytes.CS2.GameStateIntegration.Mqtt.HomeAssistant;

public record DiscoveryPayload(
    [property: JsonPropertyName("name")] string Name,
    [property: JsonPropertyName("unique_id")] string UniqueId,
    [property: JsonPropertyName("state_topic")] string StateTopic,
    [property: JsonPropertyName("value_template")] string ValueTemplate,
    [property: JsonPropertyName("icon")] string Icon,
    [property: JsonPropertyName("device")] Device Device,
    [property: JsonPropertyName("availability")] Availability Availability);