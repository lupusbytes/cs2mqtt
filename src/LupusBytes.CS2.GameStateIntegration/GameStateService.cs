using System.Collections.Concurrent;
using LupusBytes.CS2.GameStateIntegration.Contracts;
using LupusBytes.CS2.GameStateIntegration.Events;

namespace LupusBytes.CS2.GameStateIntegration;

internal sealed class GameStateService : ObservableGameState, IGameStateService
{
    private readonly ConcurrentDictionary<SteamId64, Subscription> gameStateSubscriptions = new();

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
    }

    private static void PushEvent<T>(IEnumerable<IObserver<T>> observers, T @event)
    {
        foreach (var observer in observers)
        {
            observer.OnNext(@event);
        }
    }

    private void OnCompleted(SteamId64 steamId) => gameStateSubscriptions.Remove(steamId, out _);

    private void OnMapEvent(MapEvent value) => PushEvent(MapObservers, value);

    private void OnPlayerEvent(PlayerEvent value) => PushEvent(PlayerObservers, value);

    private void OnRoundEvent(RoundEvent value) => PushEvent(RoundObservers, value);

    private sealed class Subscription : IObserver<MapEvent>, IObserver<PlayerEvent>, IObserver<RoundEvent>
    {
        private readonly GameStateService service;
        private readonly IDisposable[] subscriptions;

        public IGameState GameState { get; }

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

            service.OnCompleted(GameState.SteamId);
        }

        public void OnError(Exception error) => throw error;
    }
}