using System.Text.Json.Serialization;

namespace LupusBytes.CS2.GameStateIntegration.Mqtt.HomeAssistant;

public record SensorConfig(
    [property: JsonPropertyName("name")] string Name,
    [property: JsonPropertyName("unique_id")] string UniqueId,
    [property: JsonPropertyName("state_topic")] string StateTopic,
    [property: JsonPropertyName("value_template")] string ValueTemplate,
    [property: JsonPropertyName("icon")] string Icon,
    [property: JsonPropertyName("device")] Device Device,
    [property: JsonPropertyName("availability")] IReadOnlyCollection<Availability> Availability,
    [property: JsonPropertyName("availability_mode")] string AvailabilityMode = "all",
    [property: JsonPropertyName("state_class"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)] string? StateClass = null,
    [property: JsonPropertyName("unit_of_measurement"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)] string? UnitOfMeasurement = null);