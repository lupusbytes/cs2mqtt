using System.Text.Json.Serialization;

namespace LupusBytes.CS2.GameStateIntegration.Contracts;

public record GameStateData(
    [property: JsonPropertyName("map")] Map? Map,
    [property: JsonPropertyName("round")] Round? Round,
    [property: JsonPropertyName("player")] PlayerWithState? Player,
    [property: JsonPropertyName("provider")] Provider? Provider);