using LupusBytes.CS2.GameStateIntegration.Mqtt.Extensions;

namespace LupusBytes.CS2.GameStateIntegration.Mqtt;

public sealed class GameStateMqttPublisher : GameStateSubscriberService
{
    private readonly IMqttClient mqttClient;

    public GameStateMqttPublisher(IGameStateService gameStateService, IMqttClient mqttClient)
        : base(gameStateService)
    {
        this.mqttClient = mqttClient;

        SubscribeToPlayer(PublishAsync);
        SubscribeToPlayerState(PublishAsync);
        SubscribeToPlayerMatchStats(PublishAsync);
        SubscribeToMap(PublishAsync);
        SubscribeToRound(PublishAsync);
    }

    private Task PublishAsync<TState>(
        StateUpdateEventArgs<TState> stateUpdate,
        CancellationToken cancellationToken)
        where TState : class
        => mqttClient.PublishAsync(stateUpdate.ToMqttMessage(), cancellationToken);
}
