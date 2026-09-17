using LupusBytes.CS2.GameStateIntegration.Contracts;

namespace LupusBytes.CS2.GameStateIntegration.Benchmarks;

/// <summary>
/// The cheapest possible <see cref="IGameStateUpdateListener"/>, so that the
/// <see cref="GameState"/> benchmarks measure the state machine rather than the listener.
/// </summary>
internal sealed class CountingUpdateListener : IGameStateUpdateListener
{
    public int Count { get; private set; }

    public void OnProviderUpdated(StateUpdateEventArgs<Provider> e) => Count++;

    public void OnMapUpdated(StateUpdateEventArgs<Map> e) => Count++;

    public void OnRoundUpdated(StateUpdateEventArgs<Round> e) => Count++;

    public void OnPlayerUpdated(StateUpdateEventArgs<Player> e) => Count++;

    public void OnPlayerStateUpdated(StateUpdateEventArgs<PlayerState> e) => Count++;

    public void OnPlayerMatchStatsUpdated(StateUpdateEventArgs<PlayerMatchStats> e) => Count++;
}
