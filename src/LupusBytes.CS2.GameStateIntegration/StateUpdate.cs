using LupusBytes.CS2.GameStateIntegration.Contracts;

namespace LupusBytes.CS2.GameStateIntegration;

public record StateUpdate<TState>(SteamId64 SteamId, TState? State)
    where TState : class
{
    public bool HasState => State is not null;
}