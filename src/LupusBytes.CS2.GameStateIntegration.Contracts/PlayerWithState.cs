using System.Text.Json.Serialization;
using LupusBytes.CS2.GameStateIntegration.Contracts.Enums;

namespace LupusBytes.CS2.GameStateIntegration.Contracts;

/// <summary>
/// This record represents the "player" object provided by the CS2 gamestate integration webhook.
/// If the <see cref="Provider" /> is not alive, the values will be of the player being spectated.
/// </summary>
/// <param name="SteamId64">The SteamID64 of the current player.</param>
/// <param name="Name">The name of the current player.</param>
/// <param name="Team">The team of the current player.</param>
/// <param name="Activity">Status of the provider.</param>
public record PlayerWithState(string SteamId64, string Name, Team? Team, Activity Activity)
    : Player(SteamId64, Name, Team, Activity)
{
    /// <summary>
    /// The state of the current player.
    /// </summary>
    [JsonPropertyOrder(int.MaxValue)]
    [JsonPropertyName("state")]
    public PlayerState? State { get; init; }
}