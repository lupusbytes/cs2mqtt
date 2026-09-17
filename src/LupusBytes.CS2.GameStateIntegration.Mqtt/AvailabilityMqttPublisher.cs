using System.Threading.Tasks.Dataflow;
using LupusBytes.CS2.GameStateIntegration.Contracts;

namespace LupusBytes.CS2.GameStateIntegration.Mqtt;

public sealed class AvailabilityMqttPublisher : GameStateSubscriberService
{
    private const string ProviderAvailabilityTopicSuffix = "status";
    private const string PlayerAvailabilityTopicSuffix = "player/status";
    private const string PlayerStateAvailabilityTopicSuffix = "player-state/status";
    private const string PlayerMatchStatsAvailabilityTopicSuffix = "player-match-stats/status";
    private const string MapAvailabilityTopicSuffix = "map/status";
    private const string RoundAvailabilityTopicSuffix = "round/status";

    private readonly HashSet<SteamId64> onlineProviders = [];
    private readonly HashSet<SteamId64> onlinePlayers = [];
    private readonly HashSet<SteamId64> onlinePlayerStates = [];
    private readonly HashSet<SteamId64> onlinePlayerMatchStats = [];
    private readonly HashSet<SteamId64> onlineMaps = [];
    private readonly HashSet<SteamId64> onlineRounds = [];

    private readonly IMqttClient mqttClient;

    public AvailabilityMqttPublisher(IGameStateService gameStateService, IMqttClient mqttClient)
        : base(gameStateService)
    {
        this.mqttClient = mqttClient;

        SubscribeToProvider((e, ct) => SetAvailabilityAsync(e, onlineProviders, ProviderAvailabilityTopicSuffix, ct));
        SubscribeToPlayer((e, ct) => SetAvailabilityAsync(e, onlinePlayers, PlayerAvailabilityTopicSuffix, ct));
        SubscribeToPlayerState((e, ct) => SetAvailabilityAsync(e, onlinePlayerStates, PlayerStateAvailabilityTopicSuffix, ct));
        SubscribeToPlayerMatchStats((e, ct) => SetAvailabilityAsync(e, onlinePlayerMatchStats, PlayerMatchStatsAvailabilityTopicSuffix, ct));
        SubscribeToMap((e, ct) => SetAvailabilityAsync(e, onlineMaps, MapAvailabilityTopicSuffix, ct));
        SubscribeToRound((e, ct) => SetAvailabilityAsync(e, onlineRounds, RoundAvailabilityTopicSuffix, ct));
    }

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

    private async Task SetAvailabilityAsync<TState>(
        StateUpdateEventArgs<TState> stateUpdate,
        HashSet<SteamId64> onlineSet,
        string topicSuffix,
        CancellationToken cancellationToken)
        where TState : class
    {
        var isOnline = onlineSet.Contains(stateUpdate.SteamId);
        var shouldBeOnline = stateUpdate.HasState;

        if (shouldBeOnline == isOnline)
        {
            return;
        }

        await SetAvailability(stateUpdate.SteamId, topicSuffix, shouldBeOnline, cancellationToken);

        if (shouldBeOnline)
        {
            onlineSet.Add(stateUpdate.SteamId);
        }
        else
        {
            onlineSet.Remove(stateUpdate.SteamId);
        }
    }

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
            .Concat(onlineProviders.Select(steamId => (steamId, ProviderAvailabilityTopicSuffix)))
            .Concat(onlinePlayerStates.Select(steamId => (steamId, PlayerStateAvailabilityTopicSuffix)))
            .Concat(onlinePlayerMatchStats.Select(steamId => (steamId, PlayerMatchStatsAvailabilityTopicSuffix)))
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