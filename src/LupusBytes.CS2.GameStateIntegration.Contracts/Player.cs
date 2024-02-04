namespace LupusBytes.CS2.GameStateIntegration.Contracts;

// Omitted:
// observer_slot: int
// clan: string
public record Player(
    string SteamId,
    string Name,
    string Team,
    Activity? Activity,
    PlayerState? State)
{
    public override string ToString()
    {
        string output = $"SteamID64: {SteamId}{Environment.NewLine}" +
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
};