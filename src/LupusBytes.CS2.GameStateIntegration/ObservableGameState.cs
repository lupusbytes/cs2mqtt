using LupusBytes.CS2.GameStateIntegration.Events;

namespace LupusBytes.CS2.GameStateIntegration;

internal abstract class ObservableGameState :
    IObservable<ProviderEvent>,
    IObservable<MapEvent>,
    IObservable<PlayerEvent>,
    IObservable<PlayerStateEvent>,
    IObservable<RoundEvent>
{
    protected ISet<IObserver<MapEvent>> MapObservers { get; } = new HashSet<IObserver<MapEvent>>();
    protected ISet<IObserver<PlayerEvent>> PlayerObservers { get; } = new HashSet<IObserver<PlayerEvent>>();
    protected ISet<IObserver<PlayerStateEvent>> PlayerStateObservers { get; } = new HashSet<IObserver<PlayerStateEvent>>();
    protected ISet<IObserver<RoundEvent>> RoundObservers { get; } = new HashSet<IObserver<RoundEvent>>();
    protected ISet<IObserver<ProviderEvent>> ProviderObservers { get; } = new HashSet<IObserver<ProviderEvent>>();

    public IDisposable Subscribe(IObserver<MapEvent> observer)
    {
        MapObservers.Add(observer);
        return new Unsubscriber<MapEvent>(MapObservers, observer);
    }

    public IDisposable Subscribe(IObserver<PlayerEvent> observer)
    {
        PlayerObservers.Add(observer);
        return new Unsubscriber<PlayerEvent>(PlayerObservers, observer);
    }

    public IDisposable Subscribe(IObserver<PlayerStateEvent> observer)
    {
        PlayerStateObservers.Add(observer);
        return new Unsubscriber<PlayerStateEvent>(PlayerStateObservers, observer);
    }

    public IDisposable Subscribe(IObserver<RoundEvent> observer)
    {
        RoundObservers.Add(observer);
        return new Unsubscriber<RoundEvent>(RoundObservers, observer);
    }

    public IDisposable Subscribe(IObserver<ProviderEvent> observer)
    {
        ProviderObservers.Add(observer);
        return new Unsubscriber<ProviderEvent>(ProviderObservers, observer);
    }

    private sealed class Unsubscriber<TEvent>(ISet<IObserver<TEvent>> observers, IObserver<TEvent> observer) : IDisposable
        where TEvent : BaseEvent
    {
        public void Dispose() => observers.Remove(observer);
    }
}