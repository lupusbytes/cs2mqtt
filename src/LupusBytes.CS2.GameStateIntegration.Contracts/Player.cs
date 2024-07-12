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
    [property: JsonPropertyName("state")] PlayerState? State)
{
    public override string ToString()
    {
        var output = $"SteamID64: {SteamId64}{Environment.NewLine}" +
                     $"Name: {Name}{Environment.NewLine}" +
                     $"Team: {Team}{Environment.NewLine}" +
                     $"Activity: {Activity}{Environment.NewLine}";

        if (State is not null)
        {
            output += $"Health: {State.Health}{Environment.NewLine}" +
                      $"Flashed: {State.Flashed}{Environment.NewLine}" +
                      $"Smoked: {State.Smoked}{Environment.NewLine}" +
                      $"Kills in round: {State.RoundKills}{Environment.NewLine}";
        }

        return output;
    }
}