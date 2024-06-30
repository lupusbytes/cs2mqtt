using System.Text.Json.Serialization;

namespace LupusBytes.CS2.GameStateIntegration.Contracts;

public record Data(
    [property: JsonPropertyName("map")] Map? Map,
    [property: JsonPropertyName("round")] Round? Round,
    [property: JsonPropertyName("player")] Player? Player,
    [property: JsonPropertyName("provider")] Provider? Provider);