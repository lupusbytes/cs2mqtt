using System.Diagnostics.CodeAnalysis;
using System.Threading.Channels;
using LupusBytes.CS2.GameStateIntegration.Contracts;
using Microsoft.Extensions.Hosting;

namespace LupusBytes.CS2.GameStateIntegration.Mqtt;

/// <summary>
/// Base class for background services that consume <see cref="IGameStateService"/> events.
/// <para>
/// A derived class calls the <c>SubscribeTo*</c> methods for the state types it cares about,
/// passing the handler that should process each update. Every subscription buffers its updates on a
/// <see cref="Channel"/>, so that the event handler never blocks the thread that ingests game state data,
/// and the handler is then invoked one update at a time from the background task.
/// </para>
/// <para>
/// All subscriptions are released again by <see cref="Dispose"/>.
/// </para>
/// </summary>
public abstract class GameStateSubscriberService(IGameStateService gameStateService) : BackgroundService
{
    private static readonly BoundedChannelOptions ChannelOptions = new(1000)
    {
        SingleWriter = false,
        SingleReader = true,
        FullMode = BoundedChannelFullMode.DropOldest,
    };

    private readonly List<Func<CancellationToken, Task>> processors = [];
    private readonly List<Action> unsubscribeActions = [];

    protected void SubscribeToProvider(Func<StateUpdateEventArgs<Provider>, CancellationToken, Task> onUpdateAsync)
        => Subscribe(
            onUpdateAsync,
            handler => gameStateService.ProviderUpdated += handler,
            handler => gameStateService.ProviderUpdated -= handler);

    protected void SubscribeToMap(Func<StateUpdateEventArgs<Map>, CancellationToken, Task> onUpdateAsync)
        => Subscribe(
            onUpdateAsync,
            handler => gameStateService.MapUpdated += handler,
            handler => gameStateService.MapUpdated -= handler);

    protected void SubscribeToRound(Func<StateUpdateEventArgs<Round>, CancellationToken, Task> onUpdateAsync)
        => Subscribe(
            onUpdateAsync,
            handler => gameStateService.RoundUpdated += handler,
            handler => gameStateService.RoundUpdated -= handler);

    protected void SubscribeToPlayer(Func<StateUpdateEventArgs<Player>, CancellationToken, Task> onUpdateAsync)
        => Subscribe(
            onUpdateAsync,
            handler => gameStateService.PlayerUpdated += handler,
            handler => gameStateService.PlayerUpdated -= handler);

    protected void SubscribeToPlayerState(Func<StateUpdateEventArgs<PlayerState>, CancellationToken, Task> onUpdateAsync)
        => Subscribe(
            onUpdateAsync,
            handler => gameStateService.PlayerStateUpdated += handler,
            handler => gameStateService.PlayerStateUpdated -= handler);

    protected void SubscribeToPlayerMatchStats(Func<StateUpdateEventArgs<PlayerMatchStats>, CancellationToken, Task> onUpdateAsync)
        => Subscribe(
            onUpdateAsync,
            handler => gameStateService.PlayerMatchStatsUpdated += handler,
            handler => gameStateService.PlayerMatchStatsUpdated -= handler);

    [SuppressMessage("Major Code Smell", "S3971:\"GC.SuppressFinalize\" should not be called", Justification = "False positive")]
    public override void Dispose()
    {
        foreach (var unsubscribe in unsubscribeActions)
        {
            unsubscribe();
        }

        unsubscribeActions.Clear();

        base.Dispose();
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Processes every subscription until the service is stopped.
    /// Override to perform work before the subscriptions are processed, and invoke this implementation afterwards.
    /// </summary>
    /// <param name="stoppingToken">Triggered when the host is shutting down.</param>
    /// <returns>A task that completes when every subscription has stopped being processed.</returns>
    protected override Task ExecuteAsync(CancellationToken stoppingToken)
        => Task.WhenAll(processors.Select(processor => processor(stoppingToken)));

    private void Subscribe<TState>(
        Func<StateUpdateEventArgs<TState>, CancellationToken, Task> onUpdateAsync,
        Action<EventHandler<StateUpdateEventArgs<TState>>> subscribe,
        Action<EventHandler<StateUpdateEventArgs<TState>>> unsubscribe)
        where TState : class
    {
        var channel = Channel.CreateBounded<StateUpdateEventArgs<TState>>(ChannelOptions);

        // The event handler deliberately closes over the channel rather than over "this",
        // so that subscribing from a derived constructor does not publish a partially constructed instance.
        EventHandler<StateUpdateEventArgs<TState>> handler = (_, e) => channel.Writer.TryWrite(e);

        subscribe(handler);
        unsubscribeActions.Add(() => unsubscribe(handler));
        processors.Add(cancellationToken => ProcessChannelAsync(channel.Reader, onUpdateAsync, cancellationToken));
    }

    private static async Task ProcessChannelAsync<TState>(
        ChannelReader<StateUpdateEventArgs<TState>> channelReader,
        Func<StateUpdateEventArgs<TState>, CancellationToken, Task> onUpdateAsync,
        CancellationToken cancellationToken)
        where TState : class
    {
        await foreach (var stateUpdate in channelReader.ReadAllAsync(cancellationToken))
        {
            await onUpdateAsync(stateUpdate, cancellationToken);
        }
    }
}
