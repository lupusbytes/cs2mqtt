using System.Text.Json.Serialization;
using LupusBytes.CS2.GameStateIntegration.Contracts.Enums;

namespace LupusBytes.CS2.GameStateIntegration.Contracts;

public record Map(
    [property: JsonPropertyName("mode")] Mode? Mode,
    [property: JsonPropertyName("name")] string Name,
    [property: JsonPropertyName("phase")] MapPhase Phase,
    [property: JsonPropertyName("round")] int Round,
    [property: JsonPropertyName("team_t")] TeamMapDetails T,
    [property: JsonPropertyName("team_ct")] TeamMapDetails CT)
{
    public override string ToString() =>
        $"Map:      {Name}{Environment.NewLine}" +
        $"Mode:     {Mode}{Environment.NewLine}" +
        $"Phase:    {Phase}{Environment.NewLine}" +
        $"Round:    {Round}{Environment.NewLine}" +
        $"T Score:  {T.Score}{Environment.NewLine}" +
        $"CT Score: {CT.Score}{Environment.NewLine}";
}
