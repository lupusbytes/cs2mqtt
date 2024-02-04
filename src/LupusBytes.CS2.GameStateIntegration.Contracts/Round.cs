using System.Text.Json.Serialization;
using LupusBytes.CS2.GameStateIntegration.Contracts.Enums;

namespace LupusBytes.CS2.GameStateIntegration.Contracts;

public record Round(
    RoundPhase Phase,
    [property: JsonPropertyName("win_team")] Team? WinTeam,
    BombState? Bomb);