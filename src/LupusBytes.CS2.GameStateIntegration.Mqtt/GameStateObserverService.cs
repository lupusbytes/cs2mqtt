using System.Diagnostics.CodeAnalysis;
using System.Threading.Channels;
using LupusBytes.CS2.GameStateIntegration.Events;
using Microsoft.Extensions.Hosting;

namespace LupusBytes.CS2.GameStateIntegration.Mqtt;

public abstract class GameStateObserverService :
    BackgroundService,
    IObserver<PlayerEvent>,
    IObserver<PlayerStateEvent>,
    IObserver<MapEvent>,
    IObserver<RoundEvent>
{
    private static readonly BoundedChannelOptions ChannelOptions = new(1000)
    {
        SingleWriter = false,
        SingleReader = true,
        FullMode = BoundedChannelFullMode.DropOldest,
    };

    private readonly IDisposable[] subscriptions;
    private readonly Channel<PlayerEvent> playerChannel;
    private readonly Channel<PlayerStateEvent> playerStateChannel;
    private readonly Channel<MapEvent> mapChannel;
    private readonly Channel<RoundEvent> roundChannel;

    protected ChannelReader<PlayerEvent> PlayerChannelReader => playerChannel.Reader;
    protected ChannelReader<PlayerStateEvent> PlayerStateChannelReader => playerStateChannel.Reader;
    protected ChannelReader<MapEvent> MapChannelReader => mapChannel.Reader;
    protected ChannelReader<RoundEvent> RoundChannelReader => roundChannel.Reader;

    protected GameStateObserverService(IGameStateService gameStateService)
    {
        playerChannel = Channel.CreateBounded<PlayerEvent>(ChannelOptions);
        playerStateChannel = Channel.CreateBounded<PlayerStateEvent>(ChannelOptions);
        mapChannel = Channel.CreateBounded<MapEvent>(ChannelOptions);
        roundChannel = Channel.CreateBounded<RoundEvent>(ChannelOptions);

        subscriptions =
        [
            gameStateService.Subscribe(this as IObserver<PlayerEvent>),
            gameStateService.Subscribe(this as IObserver<PlayerStateEvent>),
            gameStateService.Subscribe(this as IObserver<RoundEvent>),
            gameStateService.Subscribe(this as IObserver<MapEvent>),
        ];
    }

    public void OnNext(PlayerEvent value) => playerChannel.Writer.TryWrite(value);
    public void OnNext(PlayerStateEvent value) => playerStateChannel.Writer.TryWrite(value);
    public void OnNext(MapEvent value) => mapChannel.Writer.TryWrite(value);
    public void OnNext(RoundEvent value) => roundChannel.Writer.TryWrite(value);

    public void OnCompleted()
    {
    }

    public void OnError(Exception error)
    {
    }

    [SuppressMessage("Major Code Smell", "S3971:\"GC.SuppressFinalize\" should not be called", Justification = "False positive")]
    public override void Dispose()
    {
        foreach (var subscription in subscriptions)
        {
            subscription.Dispose();
        }

        base.Dispose();
        GC.SuppressFinalize(this);
    }
}