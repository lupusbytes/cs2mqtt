using LupusBytes.CS2.GameStateIntegration.Contracts;

namespace LupusBytes.CS2.GameStateIntegration;

/// <summary>
/// Receives the state changes detected by a single <see cref="IGameState"/>.
/// A <see cref="StateUpdateEventArgs{TState}"/> with a <see langword="null"/> state means
/// that the block is no longer available for that SteamID.
/// </summary>
/// <remarks>
/// This is the internal seam between a per-SteamID <see cref="IGameState"/> and the
/// <see cref="IGameStateService"/> that fans its updates out to the application.
/// </remarks>
internal interface IGameStateUpdateListener
{
    void OnProviderUpdated(StateUpdateEventArgs<Provider> e);

    void OnMapUpdated(StateUpdateEventArgs<Map> e);

    void OnRoundUpdated(StateUpdateEventArgs<Round> e);

    void OnPlayerUpdated(StateUpdateEventArgs<Player> e);

    void OnPlayerStateUpdated(StateUpdateEventArgs<PlayerState> e);

    void OnPlayerMatchStatsUpdated(StateUpdateEventArgs<PlayerMatchStats> e);
}
