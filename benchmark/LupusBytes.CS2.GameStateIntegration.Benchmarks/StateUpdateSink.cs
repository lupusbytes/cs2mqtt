using LupusBytes.CS2.GameStateIntegration.Contracts;

namespace LupusBytes.CS2.GameStateIntegration.Benchmarks;

/// <summary>
/// A single consumer that subscribes to all six state update events of an
/// <see cref="IGameStateService"/> and does the cheapest possible thing with them,
/// so that the benchmarks measure the dispatch machinery rather than the consumer.
/// </summary>
internal sealed class StateUpdateSink
{
    public int Count { get; private set; }

    public void SubscribeTo(IGameStateService source)
    {
        source.ProviderUpdated += OnProviderUpdated;
        source.MapUpdated += OnMapUpdated;
        source.RoundUpdated += OnRoundUpdated;
        source.PlayerUpdated += OnPlayerUpdated;
        source.PlayerStateUpdated += OnPlayerStateUpdated;
        source.PlayerMatchStatsUpdated += OnPlayerMatchStatsUpdated;
    }

    public void UnsubscribeFrom(IGameStateService source)
    {
        source.ProviderUpdated -= OnProviderUpdated;
        source.MapUpdated -= OnMapUpdated;
        source.RoundUpdated -= OnRoundUpdated;
        source.PlayerUpdated -= OnPlayerUpdated;
        source.PlayerStateUpdated -= OnPlayerStateUpdated;
        source.PlayerMatchStatsUpdated -= OnPlayerMatchStatsUpdated;
    }

    private void OnProviderUpdated(object? sender, StateUpdateEventArgs<Provider> e) => Count++;

    private void OnMapUpdated(object? sender, StateUpdateEventArgs<Map> e) => Count++;

    private void OnRoundUpdated(object? sender, StateUpdateEventArgs<Round> e) => Count++;

    private void OnPlayerUpdated(object? sender, StateUpdateEventArgs<Player> e) => Count++;

    private void OnPlayerStateUpdated(object? sender, StateUpdateEventArgs<PlayerState> e) => Count++;

    private void OnPlayerMatchStatsUpdated(object? sender, StateUpdateEventArgs<PlayerMatchStats> e) => Count++;
}
