using System.Threading.Channels;
using LupusBytes.CS2.GameStateIntegration.Contracts;

namespace LupusBytes.CS2.GameStateIntegration.Mqtt;

/// <summary>
/// This class sets up <see cref="StateUpdate{TState}"/> subscriptions for
/// <see cref="Provider"/>,
/// <see cref="Map"/>,
/// <see cref="Round"/>,
/// <see cref="Player"/> and
/// <see cref="PlayerState"/>,
/// on the given <see cref="IGameStateService"/> and re-transmits all the incoming events on matching <see cref="Channel"/>s.
/// Use <see cref="GameStateWithoutProviderObserverService"/> if <see cref="Provider"/> is not needed.
/// </summary>
public abstract class GameStateObserverService
    : GameStateWithoutProviderObserverService, IObserver<StateUpdate<Provider>>
{
    private readonly Channel<StateUpdate<Provider>> providerChannel;
    private readonly IDisposable providerSubscription;

    protected ChannelReader<StateUpdate<Provider>> ProviderChannelReader => providerChannel.Reader;

    protected GameStateObserverService(IGameStateService gameStateService)
        : base(gameStateService)
    {
        providerChannel = Channel.CreateBounded<StateUpdate<Provider>>(ChannelOptions);
        providerSubscription = gameStateService.Subscribe(this as IObserver<StateUpdate<Provider>>);
    }

    public void OnNext(StateUpdate<Provider> value) => providerChannel.Writer.TryWrite(value);

    public override void Dispose()
    {
        providerSubscription.Dispose();
        base.Dispose();
        GC.SuppressFinalize(this);
    }
}