using System.Text.Json.Serialization;

namespace LupusBytes.CS2.GameStateIntegration.Mqtt.HomeAssistant;

public record Availability([property: JsonPropertyName("topic")] string Topic);