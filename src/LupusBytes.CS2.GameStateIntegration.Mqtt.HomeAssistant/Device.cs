using System.Text.Json.Serialization;

namespace LupusBytes.CS2.GameStateIntegration.Mqtt.HomeAssistant;

public record Device(
    [property: JsonPropertyName("name")] string Name,
    [property: JsonPropertyName("identifiers")] string Id,
    [property: JsonPropertyName("manufacturer")] string Manufacturer,
    [property: JsonPropertyName("model")] string Model,
    [property: JsonPropertyName("sw_version")] string SoftwareVersion);