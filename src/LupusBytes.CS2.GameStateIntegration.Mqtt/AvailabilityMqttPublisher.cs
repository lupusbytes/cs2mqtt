using System.Threading.Channels;
using LupusBytes.CS2.GameStateIntegration.Contracts;
using LupusBytes.CS2.GameStateIntegration.Events;

namespace LupusBytes.CS2.GameStateIntegration.Mqtt;

public sealed class AvailabilityMqttPublisher(
    IGameStateService gameStateService,
    IMqttClient mqttClient) : GameStateObserverService(gameStateService)
{
    private readonly HashSet<SteamId64> onlinePlayers = [];
    private readonly HashSet<SteamId64> onlinePlayerStates = [];
    private readonly HashSet<SteamId64> onlineMaps = [];
    private readonly HashSet<SteamId64> onlineRounds = [];

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await SetSystemAvailability(stoppingToken);

        var tasks = new[]
        {
            ProcessChannelAsync(PlayerChannelReader, onlinePlayers, ShouldBeOnline, "player/status", stoppingToken),
            ProcessChannelAsync(PlayerStateChannelReader, onlinePlayerStates, ShouldBeOnline, "player-state/status", stoppingToken),
            ProcessChannelAsync(MapChannelReader, onlineMaps, ShouldBeOnline, "map/status", stoppingToken),
            ProcessChannelAsync(RoundChannelReader, onlineRounds, ShouldBeOnline, "round/status", stoppingToken),
        };

        // This task must be awaited to prevent the subscriptions from being disposed.
        await Task.WhenAll(tasks);
    }

    private Task SetSystemAvailability(CancellationToken cancellationToken) =>
        mqttClient.PublishAsync(
            new MqttMessage
            {
                Topic = MqttConstants.SystemAvailabilityTopic,
                Payload = "online",
                RetainFlag = true,
            },
            cancellationToken);

    private async Task ProcessChannelAsync<TEvent>(
        ChannelReader<TEvent> channelReader,
        HashSet<SteamId64> onlineSet,
        Func<TEvent, bool> shouldBeOnlineFunc,
        string topicSuffix,
        CancellationToken cancellationToken)
        where TEvent : BaseEvent
    {
        while (await channelReader.WaitToReadAsync(cancellationToken))
        {
            while (channelReader.TryRead(out var @event))
            {
                var isOnline = !onlineSet.Add(@event.SteamId);
                var shouldBeOnline = shouldBeOnlineFunc(@event);

                if (shouldBeOnline == isOnline)
                {
                    continue;
                }

                await mqttClient.PublishAsync(
                    new MqttMessage
                    {
                        Topic = $"{MqttConstants.BaseTopic}/{@event.SteamId}/{topicSuffix}",
                        Payload = shouldBeOnline ? "online" : "offline",
                        RetainFlag = true,
                    },
                    cancellationToken);

                if (!shouldBeOnline)
                {
                    onlineSet.Remove(@event.SteamId);
                }
            }
        }
    }

    private static bool ShouldBeOnline(PlayerEvent @event) => @event.Player is not null;
    private static bool ShouldBeOnline(PlayerStateEvent @event) => @event.PlayerState is not null;
    private static bool ShouldBeOnline(MapEvent @event) => @event.Map is not null;
    private static bool ShouldBeOnline(RoundEvent @event) => @event.Round is not null;
}