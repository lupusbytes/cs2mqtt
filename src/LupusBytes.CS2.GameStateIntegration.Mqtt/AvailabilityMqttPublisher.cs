using System.Threading.Channels;
using System.Threading.Tasks.Dataflow;
using LupusBytes.CS2.GameStateIntegration.Contracts;

namespace LupusBytes.CS2.GameStateIntegration.Mqtt;

public sealed class AvailabilityMqttPublisher(
    IGameStateService gameStateService,
    IMqttClient mqttClient) : GameStateObserverService(gameStateService)
{
    private const string ProviderAvailabilityTopicSuffix = "status";
    private const string PlayerAvailabilityTopicSuffix = "player/status";
    private const string PlayerStateAvailabilityTopicSuffix = "player-state/status";
    private const string MapAvailabilityTopicSuffix = "map/status";
    private const string RoundAvailabilityTopicSuffix = "round/status";

    private readonly HashSet<SteamId64> onlineProviders = [];
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
            ProcessChannelAsync(ProviderChannelReader, onlineProviders, ProviderAvailabilityTopicSuffix, stoppingToken),
            ProcessChannelAsync(PlayerChannelReader, onlinePlayers, PlayerAvailabilityTopicSuffix, stoppingToken),
            ProcessChannelAsync(PlayerStateChannelReader, onlinePlayerStates, PlayerStateAvailabilityTopicSuffix, stoppingToken),
            ProcessChannelAsync(MapChannelReader, onlineMaps, MapAvailabilityTopicSuffix, stoppingToken),
            ProcessChannelAsync(RoundChannelReader, onlineRounds, RoundAvailabilityTopicSuffix, stoppingToken));

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

    private async Task ProcessChannelAsync<TState>(
        ChannelReader<StateUpdate<TState>> channelReader,
        HashSet<SteamId64> onlineSet,
        string topicSuffix,
        CancellationToken cancellationToken)
        where TState : class
    {
        await foreach (var stateUpdate in channelReader.ReadAllAsync(cancellationToken))
        {
            var isOnline = onlineSet.Contains(stateUpdate.SteamId);
            var shouldBeOnline = stateUpdate.HasState;

            if (shouldBeOnline == isOnline)
            {
                continue;
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