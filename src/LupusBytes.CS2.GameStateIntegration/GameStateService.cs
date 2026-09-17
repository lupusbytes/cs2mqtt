using System.Collections.Concurrent;
using LupusBytes.CS2.GameStateIntegration.Contracts;

namespace LupusBytes.CS2.GameStateIntegration;

internal sealed class GameStateService : IGameStateService, IGameStateUpdateListener
{
    private readonly ConcurrentDictionary<SteamId64, Connection> connections = new();
    private readonly GameStateOptions options;

    public GameStateService(GameStateOptions options)
    {
        this.options = options;

        // Start a periodic background task that will remove connections that have stopped receiving events.
        // We do not receive any explicit events from Counter-Strike that we can use to determine that a provider has been disconnected.
        // When the player quits the game, we just stop receiving events.
        // The connection timeout should be a tiny bit longer than the heartbeat defined in the gamestate_integration.cfg
        _ = CleanupDeadConnectionsAsync(
            checkInterval: TimeSpan.FromSeconds(options.TimeoutCleanupIntervalInSeconds),
            connectionTimeout: TimeSpan.FromSeconds(options.TimeoutInSeconds));
    }

    public event EventHandler<StateUpdateEventArgs<Provider>>? ProviderUpdated;

    public event EventHandler<StateUpdateEventArgs<Map>>? MapUpdated;

    public event EventHandler<StateUpdateEventArgs<Round>>? RoundUpdated;

    public event EventHandler<StateUpdateEventArgs<Player>>? PlayerUpdated;

    public event EventHandler<StateUpdateEventArgs<PlayerState>>? PlayerStateUpdated;

    public event EventHandler<StateUpdateEventArgs<PlayerMatchStats>>? PlayerMatchStatsUpdated;

    public Map? GetMap(SteamId64 steamId)
        => connections.GetValueOrDefault(steamId)?.GameState.Map;

    public PlayerData? GetPlayer(SteamId64 steamId)
        => connections.GetValueOrDefault(steamId)?.GameState.Player;

    public Round? GetRound(SteamId64 steamId)
        => connections.GetValueOrDefault(steamId)?.GameState.Round;

    public void ProcessEvent(GameStateData data)
    {
        if (data.Provider is null)
        {
            throw new ArgumentException(
                "Cannot get SteamID because provider object is missing. " +
                "Include \"provider\": \"1\" in the CS2 gamestate config",
                nameof(data));
        }

        var steamId64 = SteamId64.FromString(data.Provider.SteamId64);

        var connection = connections.GetOrAdd(
            steamId64,
            static (key, arg) => new Connection(new GameState(key, arg.options.IgnoreSpectatedPlayers, arg)),
            this);

        connection.GameState.ProcessEvent(data);
        connection.LastActivity = DateTimeOffset.UtcNow;
    }

    public void OnProviderUpdated(StateUpdateEventArgs<Provider> e) => ProviderUpdated?.Invoke(this, e);

    public void OnMapUpdated(StateUpdateEventArgs<Map> e) => MapUpdated?.Invoke(this, e);

    public void OnRoundUpdated(StateUpdateEventArgs<Round> e) => RoundUpdated?.Invoke(this, e);

    public void OnPlayerUpdated(StateUpdateEventArgs<Player> e) => PlayerUpdated?.Invoke(this, e);

    public void OnPlayerStateUpdated(StateUpdateEventArgs<PlayerState> e) => PlayerStateUpdated?.Invoke(this, e);

    public void OnPlayerMatchStatsUpdated(StateUpdateEventArgs<PlayerMatchStats> e) => PlayerMatchStatsUpdated?.Invoke(this, e);

    private async Task CleanupDeadConnectionsAsync(
        TimeSpan checkInterval,
        TimeSpan connectionTimeout)
    {
        using var timer = new PeriodicTimer(checkInterval);
        while (await timer.WaitForNextTickAsync())
        {
            var deadConnections = connections.Values
                .Where(x => DateTimeOffset.UtcNow - x.LastActivity > connectionTimeout)
                .ToList();

            foreach (var deadConnection in deadConnections)
            {
                Disconnect(deadConnection.GameState.SteamId);
            }
        }
    }

    /// <summary>
    /// The provider has been disconnected.
    /// </summary>
    /// <param name="steamId">The SteamID of the disconnected provider.</param>
    private void Disconnect(SteamId64 steamId)
    {
        // Remove the local connection
        connections.Remove(steamId, out _);

        // Send null states to all subscribers for this SteamID to overwrite their last buffer.
        OnProviderUpdated(new StateUpdateEventArgs<Provider>(steamId, state: null));
        OnMapUpdated(new StateUpdateEventArgs<Map>(steamId, state: null));
        OnRoundUpdated(new StateUpdateEventArgs<Round>(steamId, state: null));
        OnPlayerUpdated(new StateUpdateEventArgs<Player>(steamId, state: null));
        OnPlayerStateUpdated(new StateUpdateEventArgs<PlayerState>(steamId, state: null));
        OnPlayerMatchStatsUpdated(new StateUpdateEventArgs<PlayerMatchStats>(steamId, state: null));
    }

    /// <summary>
    /// A single connected Counter-Strike instance and the time we last heard from it.
    /// </summary>
    private sealed class Connection(IGameState gameState)
    {
        public IGameState GameState { get; } = gameState;

        public DateTimeOffset LastActivity { get; set; }
    }
}
