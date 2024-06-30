using System.Text.Json.Serialization;
using LupusBytes.CS2.GameStateIntegration.Contracts.Enums;

namespace LupusBytes.CS2.GameStateIntegration.Contracts;

/// <summary>
/// Information about the current round.<br/>
/// Enabled by including <code>"round" "1"</code> in the game state cfg file.
/// </summary>
/// <param name="Phase">The current round phase.</param>
/// <param name="WinTeam">The round winning team.</param>
/// <param name="Bomb">The current bomb state.</param>
public record Round(
    [property: JsonPropertyName("phase")] RoundPhase Phase,
    [property: JsonPropertyName("win_team")] Team? WinTeam,
    [property: JsonPropertyName("bomb")] BombState? Bomb);