using System.Text.Json.Serialization;
using LupusBytes.CS2.GameStateIntegration.Contracts.Enums;

namespace LupusBytes.CS2.GameStateIntegration.Contracts;

// Omitted:
// observer_slot: int
// clan: string
public record Player(
    [property: JsonPropertyName("steamid")] string SteamId64,
    [property: JsonPropertyName("name")] string Name,
    [property: JsonPropertyName("team")] Team? Team,
    [property: JsonPropertyName("activity")] Activity Activity,
    [property: JsonPropertyName("state")] PlayerState? State);