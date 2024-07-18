using LupusBytes.CS2.GameStateIntegration.Events;

namespace LupusBytes.CS2.GameStateIntegration;

internal abstract class ObservableGameState :
    IObservable<MapEvent>,
    IObservable<PlayerEvent>,
    IObservable<RoundEvent>
{
    protected ISet<IObserver<MapEvent>> MapObservers { get; } = new HashSet<IObserver<MapEvent>>();
    protected ISet<IObserver<PlayerEvent>> PlayerObservers { get; } = new HashSet<IObserver<PlayerEvent>>();
    protected ISet<IObserver<RoundEvent>> RoundObservers { get; } = new HashSet<IObserver<RoundEvent>>();

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

    public IDisposable Subscribe(IObserver<RoundEvent> observer)
    {
        RoundObservers.Add(observer);
        return new Unsubscriber<RoundEvent>(RoundObservers, observer);
    }

    private sealed class Unsubscriber<TEvent>(ISet<IObserver<TEvent>> observers, IObserver<TEvent> observer) : IDisposable
        where TEvent : BaseEvent
    {
        public void Dispose() => observers.Remove(observer);
    }
}