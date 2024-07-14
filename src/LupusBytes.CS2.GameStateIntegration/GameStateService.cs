using System.Collections.Concurrent;
using LupusBytes.CS2.GameStateIntegration.Contracts;
using LupusBytes.CS2.GameStateIntegration.Events;

namespace LupusBytes.CS2.GameStateIntegration;

internal sealed class GameStateService : ObservableGameState, IGameStateService
{
    private readonly ConcurrentDictionary<SteamId64, Subscription> gameStateSubscriptions;

    public GameStateService(GameStateOptions options)
    {
        gameStateSubscriptions = new ConcurrentDictionary<SteamId64, Subscription>();

        // Start a periodic background task that will remove subscriptions that have stopped receiving events.
        // We do not receive any explicit events from Counter-Strike that we can use to determine that a provider has been disconnected.
        // When the player quits the game, we just stop receiving events.
        // The subscription timeout should be a tiny bit longer than the heartbeat defined in the gamestate_integration.cfg
        _ = CleanupDeadSubscriptionsAsync(
            checkInterval: TimeSpan.FromSeconds(options.TimeoutCleanupIntervalInSeconds),
            subscriptionTimeout: TimeSpan.FromSeconds(options.TimeoutInSeconds));
    }

    public Map? GetMap(SteamId64 steamId)
        => gameStateSubscriptions.GetValueOrDefault(steamId)?.GameState.Map;

    public Player? GetPlayer(SteamId64 steamId)
        => gameStateSubscriptions.GetValueOrDefault(steamId)?.GameState.Player;

    public Round? GetRound(SteamId64 steamId)
        => gameStateSubscriptions.GetValueOrDefault(steamId)?.GameState.Round;

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

        var gameStateSubscription = gameStateSubscriptions.GetOrAdd(
            steamId64,
            new Subscription(this, new GameState(steamId64)));

        gameStateSubscription.GameState.ProcessEvent(data);
        gameStateSubscription.LastActivity = DateTimeOffset.UtcNow;
    }

    private static void PushEvent<T>(IEnumerable<IObserver<T>> observers, T @event)
    {
        foreach (var observer in observers)
        {
            observer.OnNext(@event);
        }
    }

    private void OnMapEvent(MapEvent value) => PushEvent(MapObservers, value);

    private void OnPlayerEvent(PlayerEvent value) => PushEvent(PlayerObservers, value);

    private void OnRoundEvent(RoundEvent value) => PushEvent(RoundObservers, value);

    private async Task CleanupDeadSubscriptionsAsync(
        TimeSpan checkInterval,
        TimeSpan subscriptionTimeout)
    {
        using var timer = new PeriodicTimer(checkInterval);
        while (await timer.WaitForNextTickAsync())
        {
            var deadSubscriptions = gameStateSubscriptions.Values
                .Where(x => DateTimeOffset.UtcNow - x.LastActivity > subscriptionTimeout)
                .ToList();

            foreach (var deadSubscription in deadSubscriptions)
            {
                deadSubscription.OnCompleted();
            }
        }
    }

    /// <summary>
    /// The provider has been disconnected.
    /// </summary>
    /// <param name="steamId">The SteamID of the disconnected provider.</param>
    private void OnDisconnectEvent(SteamId64 steamId)
    {
        // Remove the local subscription
        gameStateSubscriptions.Remove(steamId, out _);

        // Send null events to all observers for this SteamID to overwrite their last buffer.
        PushEvent(MapObservers, new MapEvent(steamId, Map: null));
        PushEvent(RoundObservers, new RoundEvent(steamId, Round: null));
        PushEvent(PlayerObservers, new PlayerEvent(steamId, Player: null));
    }

    private sealed class Subscription : IObserver<MapEvent>, IObserver<PlayerEvent>, IObserver<RoundEvent>
    {
        private readonly GameStateService service;
        private readonly IDisposable[] subscriptions;

        public IGameState GameState { get; }
        public DateTimeOffset LastActivity { get; set; }

        public Subscription(GameStateService service, IGameState gameState)
        {
            this.service = service;
            GameState = gameState;
            var mapSub = GameState.Subscribe(this as IObserver<MapEvent>);
            var roundSub = GameState.Subscribe(this as IObserver<RoundEvent>);
            var playerSub = GameState.Subscribe(this as IObserver<PlayerEvent>);
            subscriptions = [mapSub, roundSub, playerSub];
        }

        public void OnNext(MapEvent value) => service.OnMapEvent(value);

        public void OnNext(PlayerEvent value) => service.OnPlayerEvent(value);

        public void OnNext(RoundEvent value) => service.OnRoundEvent(value);

        public void OnCompleted()
        {
            foreach (var subscription in subscriptions)
            {
                subscription.Dispose();
            }

            service.OnDisconnectEvent(GameState.SteamId);
        }

        public void OnError(Exception error) => throw error;
    }
}