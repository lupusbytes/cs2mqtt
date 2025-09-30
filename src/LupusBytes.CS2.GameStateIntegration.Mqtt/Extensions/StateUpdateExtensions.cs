using System.Text.Json;
using LupusBytes.CS2.GameStateIntegration.Contracts;

namespace LupusBytes.CS2.GameStateIntegration.Mqtt.Extensions;

public static class StateUpdateExtensions
{
    private static readonly Dictionary<Type, string> TypeLevel = new()
    {
        { typeof(Provider), "provider" },
        { typeof(Map), "map" },
        { typeof(Round), "round" },
        { typeof(Player), "player" },
        { typeof(PlayerState), "player-state" },
    };

    public static MqttMessage ToMqttMessage<TState>(this StateUpdate<TState> stateUpdate)
        where TState : class
    {
        var topic = $"{MqttConstants.BaseTopic}/{stateUpdate.SteamId}/{TypeLevel[typeof(TState)]}";

        var payload = stateUpdate.HasState
            ? JsonSerializer.Serialize(stateUpdate.State)
            : string.Empty;

        return new MqttMessage
        {
            Topic = topic,
            Payload = payload,
            RetainFlag = true,
        };
    }
}