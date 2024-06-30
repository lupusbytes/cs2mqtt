using System.Text.Json.Serialization;

namespace LupusBytes.CS2.GameStateIntegration.Contracts;

public record Provider(
    [property: JsonPropertyName("name")] string Name,
    [property: JsonPropertyName("appid")] int AppId,
    [property: JsonPropertyName("version")] int Version,
    [property: JsonPropertyName("steamid")] string SteamId,
    [property: JsonPropertyName("timestamp")] int Timestamp);