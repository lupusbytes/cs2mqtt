using System.Text.Json.Serialization;
using LupusBytes.CS2.GameStateIntegration.Contracts.Enums;

namespace LupusBytes.CS2.GameStateIntegration.Contracts;

// Omitted:
// "num_matches_to_win_series": 0
// "current_spectators": 0
// "souvenirs_total": 0
public record Map(
    Mode? Mode,
    string Name,
    MapPhase Phase,
    int Round,
    [property: JsonPropertyName("team_t")] TeamMapDetails T,
    [property: JsonPropertyName("team_ct")] TeamMapDetails CT)
{
    public override string ToString()
    {
        return $"Map:      {Name}{Environment.NewLine}" +
               $"Mode:     {Mode}{Environment.NewLine}" +
               $"Phase:    {Phase}{Environment.NewLine}" +
               $"Round:    {Round}{Environment.NewLine}" +
               $"T Score:  {T.Score}{Environment.NewLine}" +
               $"CT Score: {CT.Score}{Environment.NewLine}";
    }
};