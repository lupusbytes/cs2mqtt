using LupusBytes.CS2.GameStateIntegration.Contracts;

namespace LupusBytes.CS2.GameStateIntegration.Api.Events;

public class RoundEventArgs(Round? round) : EventArgs
{
    public Round? Round { get; } = round;
}
