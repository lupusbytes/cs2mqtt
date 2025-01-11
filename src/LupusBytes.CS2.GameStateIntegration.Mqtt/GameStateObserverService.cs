using System.Threading.Channels;
using LupusBytes.CS2.GameStateIntegration.Events;

namespace LupusBytes.CS2.GameStateIntegration.Mqtt;

/// <summary>
/// This class sets up subscriptions for
/// <see cref="PlayerEvent"/>,
/// <see cref="PlayerStateEvent"/>,
/// <see cref="MapEvent"/>,
/// <see cref="RoundEvent"/> and
/// <see cref="ProviderEvent"/>,
/// on the given <see cref="IGameStateService"/> and re-transmits all the incoming events on matching <see cref="Channel"/>s.
/// Use <see cref="GameStateWithoutProviderObserverService"/> if <see cref="ProviderEvent"/> is not needed.
/// </summary>
public abstract class GameStateObserverService
    : GameStateWithoutProviderObserverService, IObserver<ProviderEvent>
{
    private readonly Channel<ProviderEvent> providerChannel;
    private readonly IDisposable providerSubscription;

    protected ChannelReader<ProviderEvent> ProviderChannelReader => providerChannel.Reader;

    protected GameStateObserverService(IGameStateService gameStateService)
        : base(gameStateService)
    {
        providerChannel = Channel.CreateBounded<ProviderEvent>(ChannelOptions);
        providerSubscription = gameStateService.Subscribe(this as IObserver<ProviderEvent>);
    }

    public void OnNext(ProviderEvent value) => providerChannel.Writer.TryWrite(value);

    public override void Dispose()
    {
        providerSubscription.Dispose();
        base.Dispose();
        GC.SuppressFinalize(this);
    }
}