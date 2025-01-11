using System.Threading.Channels;
using System.Threading.Tasks.Dataflow;
using LupusBytes.CS2.GameStateIntegration.Contracts;
using LupusBytes.CS2.GameStateIntegration.Events;

namespace LupusBytes.CS2.GameStateIntegration.Mqtt;

public sealed class AvailabilityMqttPublisher(
    IGameStateService gameStateService,
    IMqttClient mqttClient) : GameStateObserverService(gameStateService)
{
    private const string PlayerAvailabilityTopicSuffix = "player/status";
    private const string PlayerStateAvailabilityTopicSuffix = "player-state/status";
    private const string MapAvailabilityTopicSuffix = "map/status";
    private const string RoundAvailabilityTopicSuffix = "round/status";

    private readonly HashSet<SteamId64> onlinePlayers = [];
    private readonly HashSet<SteamId64> onlinePlayerStates = [];
    private readonly HashSet<SteamId64> onlineMaps = [];
    private readonly HashSet<SteamId64> onlineRounds = [];

    public override async Task StartAsync(CancellationToken cancellationToken)
    {
        await SetSystemAvailability(online: true, cancellationToken);
        await base.StartAsync(cancellationToken);
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        // Invoke base.StopAsync first to stop the background task that processes the different channels.
        // This order is necessary to prevent the HashSet collections from being modified and new online messages from being published,
        // while SetAllOffline iterates the collections and attempts to publish offline messages.
        await base.StopAsync(cancellationToken);

        await SetAllOffline(cancellationToken);
        await SetSystemAvailability(online: false, cancellationToken);
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
        => Task.WhenAll(
            ProcessChannelAsync(PlayerChannelReader, onlinePlayers, PlayerAvailabilityTopicSuffix, ShouldBeOnline, stoppingToken),
            ProcessChannelAsync(PlayerStateChannelReader, onlinePlayerStates, PlayerStateAvailabilityTopicSuffix, ShouldBeOnline, stoppingToken),
            ProcessChannelAsync(MapChannelReader, onlineMaps, MapAvailabilityTopicSuffix, ShouldBeOnline, stoppingToken),
            ProcessChannelAsync(RoundChannelReader, onlineRounds, RoundAvailabilityTopicSuffix, ShouldBeOnline, stoppingToken));

    private Task SetSystemAvailability(bool online, CancellationToken cancellationToken)
        => mqttClient.PublishAsync(
            new MqttMessage
            {
                Topic = MqttConstants.SystemAvailabilityTopic,
                Payload = online ? "online" : "offline",
                RetainFlag = true,
            },
            cancellationToken);

    private Task SetAvailability(
        SteamId64 steamId,
        string topicSuffix,
        bool online,
        CancellationToken cancellationToken)
        => mqttClient.PublishAsync(
            new MqttMessage
            {
                Topic = $"{MqttConstants.BaseTopic}/{steamId}/{topicSuffix}",
                Payload = online ? "online" : "offline",
                RetainFlag = true,
            },
            cancellationToken);

    private async Task ProcessChannelAsync<TEvent>(
        ChannelReader<TEvent> channelReader,
        HashSet<SteamId64> onlineSet,
        string topicSuffix,
        Func<TEvent, bool> shouldBeOnlineFunc,
        CancellationToken cancellationToken)
        where TEvent : BaseEvent
    {
        await foreach (var @event in channelReader.ReadAllAsync(cancellationToken))
        {
            var isOnline = !onlineSet.Add(@event.SteamId);
            var shouldBeOnline = shouldBeOnlineFunc(@event);

            if (shouldBeOnline == isOnline)
            {
                continue;
            }

            await SetAvailability(@event.SteamId, topicSuffix, shouldBeOnline, cancellationToken);

            if (!shouldBeOnline)
            {
                onlineSet.Remove(@event.SteamId);
            }
        }
    }

    private static bool ShouldBeOnline(PlayerEvent @event) => @event.Player is not null;
    private static bool ShouldBeOnline(PlayerStateEvent @event) => @event.PlayerState is not null;
    private static bool ShouldBeOnline(MapEvent @event) => @event.Map is not null;
    private static bool ShouldBeOnline(RoundEvent @event) => @event.Round is not null;

    private Task SetAllOffline(CancellationToken cancellationToken)
    {
        var workerBlock = new ActionBlock<(SteamId64 SteamId, string TopicSuffix)>(
            x => SetAvailability(x.SteamId, x.TopicSuffix, online: false, cancellationToken),
            new ExecutionDataflowBlockOptions
            {
                // Specify that any number of entries may be processed concurrently,
                // with the maximum automatically managed by the underlying scheduler.
                MaxDegreeOfParallelism = DataflowBlockOptions.Unbounded,
                EnsureOrdered = false,
                CancellationToken = cancellationToken,
            });

        // Combine all sets into a single sequence with their respective topic suffixes
        var entries = onlinePlayers.Select(steamId => (steamId, PlayerAvailabilityTopicSuffix))
            .Concat(onlinePlayerStates.Select(steamId => (steamId, PlayerStateAvailabilityTopicSuffix)))
            .Concat(onlineMaps.Select(steamId => (steamId, MapAvailabilityTopicSuffix)))
            .Concat(onlineRounds.Select(steamId => (steamId, RoundAvailabilityTopicSuffix)));

        foreach (var entry in entries)
        {
            workerBlock.Post(entry);
        }

        workerBlock.Complete();

        return workerBlock.Completion;
    }
}