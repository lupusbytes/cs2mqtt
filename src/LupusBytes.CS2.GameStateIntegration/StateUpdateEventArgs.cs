using LupusBytes.CS2.GameStateIntegration.Contracts;

namespace LupusBytes.CS2.GameStateIntegration;

/// <summary>
/// The arguments of a game state update event.
/// </summary>
/// <typeparam name="TState">The type of the game state block that was updated.</typeparam>
/// <param name="steamId">The SteamID of the game state provider the update belongs to.</param>
/// <param name="state">The new state, or <see langword="null"/> if the state is no longer available.</param>
public sealed class StateUpdateEventArgs<TState>(SteamId64 steamId, TState? state) : EventArgs
    where TState : class
{
    public SteamId64 SteamId { get; } = steamId;

    public TState? State { get; } = state;

    public bool HasState => State is not null;
}
