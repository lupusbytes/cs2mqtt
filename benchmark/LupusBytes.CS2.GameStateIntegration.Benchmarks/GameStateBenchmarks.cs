using BenchmarkDotNet.Attributes;
using LupusBytes.CS2.GameStateIntegration.Contracts;

namespace LupusBytes.CS2.GameStateIntegration.Benchmarks;

/// <summary>
/// Measures a single <see cref="GameState"/>: comparing the incoming payload against the
/// cached state and notifying its listener about whatever changed.
/// A <see cref="GameState"/> always has exactly one listener - the owning
/// <see cref="GameStateService"/> - so there is nothing to fan out here.
/// </summary>
[MemoryDiagnoser]
public class GameStateBenchmarks
{
    private GameState gameState = null!;
    private GameStateData[] changing = null!;
    private GameStateData unchanged = null!;
    private int index;

    [GlobalSetup]
    public void Setup()
    {
        var steamId = GameStateDataFactory.SteamId(0);
        gameState = new GameState(steamId, ignoreSpectatedPlayers: false, new CountingUpdateListener());

        changing = [GameStateDataFactory.Create(steamId, seed: 1), GameStateDataFactory.Create(steamId, seed: 2)];
        unchanged = GameStateDataFactory.Create(steamId, seed: 3);

        // Prime the state so that the "unchanged" benchmark really does hit the no-op path.
        gameState.ProcessEvent(unchanged);
    }

    /// <summary>
    /// Every block differs from the previous event, so all six state types are pushed to the listener.
    /// </summary>
    [Benchmark]
    public void ProcessEventWithAllStatesChanged() => gameState.ProcessEvent(changing[index++ & 1]);

    /// <summary>
    /// Nothing but the provider changes, so only the provider update is pushed.
    /// This is by far the most common case in production, where CS2 sends a full payload on every heartbeat.
    /// </summary>
    [Benchmark]
    public void ProcessEventWithNoStatesChanged() => gameState.ProcessEvent(unchanged);
}
