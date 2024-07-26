using System.Runtime.CompilerServices;
using System.Text.Json;
using LupusBytes.CS2.GameStateIntegration.Events;

namespace LupusBytes.CS2.GameStateIntegration.Mqtt.Extensions;

public static class EventExtensions
{
    public static MqttMessage ToMqttMessage(this BaseEvent @event)
    {
        string topic;
        string payload;
        switch (@event)
        {
            case MapEvent m:
                topic = $"{MqttConstants.BaseTopic}/{@event.SteamId}/map";
                payload = m.Map is null ? string.Empty : JsonSerializer.Serialize(m.Map);
                break;
            case PlayerEvent p:
                topic = $"{MqttConstants.BaseTopic}/{@event.SteamId}/player";
                payload = p.Player is null ? string.Empty : JsonSerializer.Serialize(p.Player);
                break;
            case PlayerStateEvent ps:
                topic = $"{MqttConstants.BaseTopic}/{@event.SteamId}/player-state";
                payload = ps.PlayerState is null ? string.Empty : JsonSerializer.Serialize(ps.PlayerState);
                break;
            case RoundEvent r:
                topic = $"{MqttConstants.BaseTopic}/{@event.SteamId}/round";
                payload = r.Round is null ? string.Empty : JsonSerializer.Serialize(r.Round);
                break;
            default: throw new SwitchExpressionException();
        }

        return new MqttMessage
        {
            Payload = payload,
            Topic = topic,
            RetainFlag = true,
        };
    }
}