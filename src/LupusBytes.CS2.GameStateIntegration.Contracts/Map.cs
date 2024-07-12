using System.Text.Json.Serialization;
using LupusBytes.CS2.GameStateIntegration.Contracts.Enums;

namespace LupusBytes.CS2.GameStateIntegration.Contracts;

public record Map(
    [property: JsonPropertyName("mode")] Mode? Mode,
    [property: JsonPropertyName("name")] string Name,
    [property: JsonPropertyName("phase")] MapPhase Phase,
    [property: JsonPropertyName("round")] int Round,
    [property: JsonPropertyName("team_t")] TeamMapDetails T,
    [property: JsonPropertyName("team_ct")] TeamMapDetails CT);
