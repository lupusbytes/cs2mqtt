using System.Diagnostics.CodeAnalysis;
using System.Threading.Channels;
using LupusBytes.CS2.GameStateIntegration.Contracts;
using Microsoft.Extensions.Hosting;

namespace LupusBytes.CS2.GameStateIntegration.Mqtt;

/// <summary>
/// This class sets up <see cref="StateUpdate{TState}"/> subscriptions for
/// <see cref="Map"/>,
/// <see cref="Round"/>,
/// <see cref="Player"/> and
/// <see cref="PlayerState"/>,
/// on the given <see cref="IGameStateService"/> and re-transmits all the incoming events on matching <see cref="Channel"/>s.
/// This class does not subscribe to <see cref="Provider"/>s.
/// Use <see cref="GameStateObserverService"/> if <see cref="Provider"/> is needed.
/// </summary>
public abstract class GameStateWithoutProviderObserverService :
    BackgroundService,
    IObserver<StateUpdate<Player>>,
    IObserver<StateUpdate<PlayerState>>,
    IObserver<StateUpdate<Map>>,
    IObserver<StateUpdate<Round>>
{
    private readonly IDisposable[] subscriptions;
    private readonly Channel<StateUpdate<Player>> playerChannel;
    private readonly Channel<StateUpdate<PlayerState>> playerStateChannel;
    private readonly Channel<StateUpdate<Map>> mapChannel;
    private readonly Channel<StateUpdate<Round>> roundChannel;

    protected static readonly BoundedChannelOptions ChannelOptions = new(1000)
    {
        SingleWriter = false,
        SingleReader = true,
        FullMode = BoundedChannelFullMode.DropOldest,
    };

    protected ChannelReader<StateUpdate<Player>> PlayerChannelReader => playerChannel.Reader;
    protected ChannelReader<StateUpdate<PlayerState>> PlayerStateChannelReader => playerStateChannel.Reader;
    protected ChannelReader<StateUpdate<Map>> MapChannelReader => mapChannel.Reader;
    protected ChannelReader<StateUpdate<Round>> RoundChannelReader => roundChannel.Reader;

    protected GameStateWithoutProviderObserverService(IGameStateService gameStateService)
    {
        playerChannel = Channel.CreateBounded<StateUpdate<Player>>(ChannelOptions);
        playerStateChannel = Channel.CreateBounded<StateUpdate<PlayerState>>(ChannelOptions);
        mapChannel = Channel.CreateBounded<StateUpdate<Map>>(ChannelOptions);
        roundChannel = Channel.CreateBounded<StateUpdate<Round>>(ChannelOptions);

        subscriptions =
        [
            gameStateService.Subscribe(this as IObserver<StateUpdate<Player>>),
            gameStateService.Subscribe(this as IObserver<StateUpdate<PlayerState>>),
            gameStateService.Subscribe(this as IObserver<StateUpdate<Round>>),
            gameStateService.Subscribe(this as IObserver<StateUpdate<Map>>),
        ];
    }

    public void OnNext(StateUpdate<Player> value) => playerChannel.Writer.TryWrite(value);
    public void OnNext(StateUpdate<PlayerState> value) => playerStateChannel.Writer.TryWrite(value);
    public void OnNext(StateUpdate<Map> value) => mapChannel.Writer.TryWrite(value);
    public void OnNext(StateUpdate<Round> value) => roundChannel.Writer.TryWrite(value);

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