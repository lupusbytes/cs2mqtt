using System.Text.Json.Serialization;
using LupusBytes.CS2.GameStateIntegration.Contracts.Enums;

namespace LupusBytes.CS2.GameStateIntegration.Contracts;

/// <summary>
/// Information about the current player.
/// If the <see cref="Provider" /> is not alive, the values will be of the player being spectated.
/// </summary>
/// <param name="SteamId64">The SteamID64 of the current player.</param>
/// <param name="Name">The name of the current player.</param>
/// <param name="Team">The team of the current player.</param>
/// <param name="Activity">Status of the provider.</param>
public record Player(
    [property: JsonPropertyName("steamid")] string SteamId64,
    [property: JsonPropertyName("name")] string Name,
    [property: JsonPropertyName("team")] Team? Team,
    [property: JsonPropertyName("activity")] Activity Activity);