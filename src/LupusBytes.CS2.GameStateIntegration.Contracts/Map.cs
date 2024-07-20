using System.Text.Json.Serialization;
using LupusBytes.CS2.GameStateIntegration.Contracts.Enums;

namespace LupusBytes.CS2.GameStateIntegration.Contracts;

/// <summary>
/// Information about the Map.
/// Enabled by including <code>"map": "1"</code> in the game state cfg file.
/// </summary>
/// <param name="Mode">The game mode.</param>
/// <param name="Name">The map name.</param>
/// <param name="Phase">The current map phase.</param>
/// <param name="Round">The current round number.</param>
public record Map(
    [property: JsonPropertyName("mode")] Mode? Mode,
    [property: JsonPropertyName("name")] string Name,
    [property: JsonPropertyName("phase")] MapPhase Phase,
    [property: JsonPropertyName("round")] int Round,
    [property: JsonPropertyName("team_t")] TeamMapDetails T,
    [property: JsonPropertyName("team_ct")] TeamMapDetails CT);
