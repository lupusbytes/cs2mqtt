using System.Text.Json.Serialization;

namespace LupusBytes.CS2.GameStateIntegration.Contracts;

public record Round(
    RoundPhase Phase,
    [property: JsonPropertyName("win_team")] Team? WinTeam,
    BombState? Bomb);