using System.Collections.Frozen;
using System.Text.Json;
using LupusBytes.CS2.GameStateIntegration.Contracts;

namespace LupusBytes.CS2.GameStateIntegration.Mqtt.Extensions;

public static class StateUpdateExtensions
{
    private static readonly FrozenDictionary<Type, string> TypeLevel = new[]
    {
        KeyValuePair.Create(typeof(Provider), "provider"),
        KeyValuePair.Create(typeof(Map), "map"),
        KeyValuePair.Create(typeof(Round), "round"),
        KeyValuePair.Create(typeof(Player), "player"),
        KeyValuePair.Create(typeof(PlayerState), "player-state"),
    }.ToFrozenDictionary();

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