using System.Collections.Concurrent;
using LupusBytes.CS2.GameStateIntegration.Contracts;

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

    public PlayerWithState? GetPlayer(SteamId64 steamId)
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
            static (key, arg) => new Subscription(arg, new GameState(key)),
            this);

        gameStateSubscription.GameState.ProcessEvent(data);
        gameStateSubscription.LastActivity = DateTimeOffset.UtcNow;
    }

    private static void PushState<TState>(
        IEnumerable<IObserver<StateUpdate<TState>>> observers,
        StateUpdate<TState> stateUpdate)
        where TState : class
    {
        foreach (var observer in observers)
        {
            observer.OnNext(stateUpdate);
        }
    }

    private void OnStateUpdate(StateUpdate<Provider> stateUpdate) => PushStateUpdate(ProviderObservers, stateUpdate);

    private void OnStateUpdate(StateUpdate<Map> value) => PushStateUpdate(MapObservers, value);

    private void OnStateUpdate(StateUpdate<Player> value) => PushStateUpdate(PlayerObservers, value);

    private void OnStateUpdate(StateUpdate<PlayerState> value) => PushStateUpdate(PlayerStateObservers, value);

    private void OnStateUpdate(StateUpdate<Round> value) => PushStateUpdate(RoundObservers, value);

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

        // Send null states to all observers for this SteamID to overwrite their last buffer.
        PushState(ProviderObservers, new StateUpdate<Provider>(steamId, State: null));
        PushState(MapObservers, new StateUpdate<Map>(steamId, State: null));
        PushState(RoundObservers, new StateUpdate<Round>(steamId, State: null));
        PushState(PlayerObservers, new StateUpdate<Player>(steamId, State: null));
        PushState(PlayerStateObservers, new StateUpdate<PlayerState>(steamId, State: null));
    }

    private sealed class Subscription :
        IObserver<StateUpdate<Provider>>,
        IObserver<StateUpdate<Map>>,
        IObserver<StateUpdate<Round>>,
        IObserver<StateUpdate<Player>>,
        IObserver<StateUpdate<PlayerState>>
    {
        private readonly GameStateService service;
        private readonly IDisposable[] subscriptions;

        public IGameState GameState { get; }
        public DateTimeOffset LastActivity { get; set; }

        public Subscription(GameStateService service, IGameState gameState)
        {
            this.service = service;
            GameState = gameState;
            var providerSub = GameState.Subscribe(this as IObserver<StateUpdate<Provider>>);
            var mapSub = GameState.Subscribe(this as IObserver<StateUpdate<Map>>);
            var roundSub = GameState.Subscribe(this as IObserver<StateUpdate<Round>>);
            var playerSub = GameState.Subscribe(this as IObserver<StateUpdate<Player>>);
            var playerStateSub = GameState.Subscribe(this as IObserver<StateUpdate<PlayerState>>);
            subscriptions = [providerSub, mapSub, roundSub, playerSub, playerStateSub];
        }

        public void OnNext(StateUpdate<Provider> value) => service.OnStateUpdate(value);

        public void OnNext(StateUpdate<Map> value) => service.OnStateUpdate(value);

        public void OnNext(StateUpdate<Round> value) => service.OnStateUpdate(value);

        public void OnNext(StateUpdate<Player> value) => service.OnStateUpdate(value);

        public void OnNext(StateUpdate<PlayerState> value) => service.OnStateUpdate(value);

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