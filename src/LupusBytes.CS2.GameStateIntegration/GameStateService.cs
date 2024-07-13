using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using LupusBytes.CS2.GameStateIntegration.Contracts;
using LupusBytes.CS2.GameStateIntegration.Events;

namespace LupusBytes.CS2.GameStateIntegration;

[SuppressMessage("Reliability", "CA2000:Dispose objects before losing scope", Justification = "TODO")]
public sealed class GameStateService :
    ObservableGameState,
    IObserver<MapEvent>,
    IObserver<PlayerEvent>,
    IObserver<RoundEvent>
{
    private readonly ConcurrentDictionary<SteamId64, IGameState> gameStates = new();

    public Map? GetMap(SteamId64 steamId)
        => gameStates.GetValueOrDefault(steamId)?.Map;

    public Player? GetPlayer(SteamId64 steamId)
        => gameStates.GetValueOrDefault(steamId)?.Player;

    public Round? GetRound(SteamId64 steamId)
        => gameStates.GetValueOrDefault(steamId)?.Round;

    void IObserver<MapEvent>.OnCompleted()
    {
        foreach (var observer in MapObservers)
        {
            observer.OnCompleted();
        }
    }

    void IObserver<PlayerEvent>.OnCompleted()
    {
        foreach (var observer in PlayerObservers)
        {
            observer.OnCompleted();
        }
    }

    void IObserver<RoundEvent>.OnCompleted()
    {
        foreach (var observer in RoundObservers)
        {
            observer.OnCompleted();
        }
    }

    void IObserver<MapEvent>.OnError(Exception error)
    {
        foreach (var observer in MapObservers)
        {
            observer.OnError(error);
        }
    }

    void IObserver<PlayerEvent>.OnError(Exception error)
    {
        foreach (var observer in PlayerObservers)
        {
            observer.OnError(error);
        }
    }

    void IObserver<RoundEvent>.OnError(Exception error)
    {
        foreach (var observer in RoundObservers)
        {
            observer.OnError(error);
        }
    }

    public void OnNext(MapEvent value)
    {
        foreach (var observer in MapObservers)
        {
            observer.OnNext(value);
        }
    }

    public void OnNext(PlayerEvent value)
    {
        foreach (var observer in PlayerObservers)
        {
            observer.OnNext(value);
        }
    }

    public void OnNext(RoundEvent value)
    {
        foreach (var observer in RoundObservers)
        {
            observer.OnNext(value);
        }
    }

    public void ProcessEvent(GameStateData @event)
    {
        if (@event.Player is null && @event.Provider is null)
        {
            throw new ArgumentException(
                "Cannot get SteamID64 because player and provider are null. " +
                "Include at least one of \"player\": \"1\" or \"provider\": \"1\" in the CS2 gamestate config",
                nameof(@event));
        }

        var steamId64 = SteamId64.FromString(@event.Player?.SteamId64 ?? @event.Provider?.SteamId64);

        var gameState = gameStates.GetOrAdd(steamId64, CreateAndSubscribeToGameState);
        gameState.ProcessEvent(@event);
    }

    private GameState CreateAndSubscribeToGameState(SteamId64 steamId64)
    {
        var gs = new GameState();
        gs.Subscribe(this as IObserver<MapEvent>);
        gs.Subscribe(this as IObserver<PlayerEvent>);
        gs.Subscribe(this as IObserver<RoundEvent>);
        return gs;
    }
}