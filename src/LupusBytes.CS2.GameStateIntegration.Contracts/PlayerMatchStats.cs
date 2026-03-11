using System.Text.Json.Serialization;

namespace LupusBytes.CS2.GameStateIntegration.Contracts;

public record PlayerMatchStats(
    [property: JsonPropertyName("kills")] int Kills,
    [property: JsonPropertyName("assists")] int Assists,
    [property: JsonPropertyName("deaths")] int Deaths,
    [property: JsonPropertyName("mvps")] int Mvps,
    [property: JsonPropertyName("score")] int Score);