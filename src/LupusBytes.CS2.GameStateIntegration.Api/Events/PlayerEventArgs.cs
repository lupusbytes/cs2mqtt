using LupusBytes.CS2.GameStateIntegration.Contracts;

namespace LupusBytes.CS2.GameStateIntegration.Api.Events;

public class PlayerEventArgs(Player player) : EventArgs
{
    public Player Player { get; } = player;
}