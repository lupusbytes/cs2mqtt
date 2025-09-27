using LupusBytes.CS2.GameStateIntegration.Contracts;

namespace LupusBytes.CS2.GameStateIntegration;

internal abstract class ObservableGameState :
    IObservable<StateUpdate<Provider>>,
    IObservable<StateUpdate<Map>>,
    IObservable<StateUpdate<Round>>,
    IObservable<StateUpdate<Player>>,
    IObservable<StateUpdate<PlayerState>>
{
    protected ISet<IObserver<StateUpdate<Provider>>> ProviderObservers { get; } = new HashSet<IObserver<StateUpdate<Provider>>>();
    protected ISet<IObserver<StateUpdate<Map>>> MapObservers { get; } = new HashSet<IObserver<StateUpdate<Map>>>();
    protected ISet<IObserver<StateUpdate<Round>>> RoundObservers { get; } = new HashSet<IObserver<StateUpdate<Round>>>();
    protected ISet<IObserver<StateUpdate<Player>>> PlayerObservers { get; } = new HashSet<IObserver<StateUpdate<Player>>>();
    protected ISet<IObserver<StateUpdate<PlayerState>>> PlayerStateObservers { get; } = new HashSet<IObserver<StateUpdate<PlayerState>>>();

    public IDisposable Subscribe(IObserver<StateUpdate<Provider>> observer)
        => Subscribe(ProviderObservers, observer);

    public IDisposable Subscribe(IObserver<StateUpdate<Map>> observer)
        => Subscribe(MapObservers, observer);

    public IDisposable Subscribe(IObserver<StateUpdate<Round>> observer)
        => Subscribe(RoundObservers, observer);

    public IDisposable Subscribe(IObserver<StateUpdate<Player>> observer)
        => Subscribe(PlayerObservers, observer);

    public IDisposable Subscribe(IObserver<StateUpdate<PlayerState>> observer)
        => Subscribe(PlayerStateObservers, observer);

    private static Unsubscriber<StateUpdate<TState>> Subscribe<TState>(
        ISet<IObserver<StateUpdate<TState>>> observers,
        IObserver<StateUpdate<TState>> observer)
        where TState : class
    {
        observers.Add(observer);
        return new Unsubscriber<StateUpdate<TState>>(observers, observer);
    }

    protected static void PushStateUpdate<TState>(
        ISet<IObserver<StateUpdate<TState>>> observers,
        StateUpdate<TState> stateUpdate)
        where TState : class
    {
        foreach (var observer in observers)
        {
            observer.OnNext(stateUpdate);
        }
    }

    private sealed class Unsubscriber<TState>(
        ISet<IObserver<TState>> observers,
        IObserver<TState> observer) : IDisposable
    {
        public void Dispose() => observers.Remove(observer);
    }
}
