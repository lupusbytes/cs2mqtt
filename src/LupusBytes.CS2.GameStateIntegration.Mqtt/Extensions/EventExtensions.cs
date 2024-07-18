using System.Runtime.CompilerServices;
using System.Text.Json;
using LupusBytes.CS2.GameStateIntegration.Events;
using MQTTnet;

namespace LupusBytes.CS2.GameStateIntegration.Mqtt.Extensions;

public static class EventExtensions
{
    public static MqttApplicationMessage ToMqttMessage(this BaseEvent @event)
    {
        string topic;
        string payload;
        switch (@event)
        {
            case MapEvent m:
                topic = $"{GameStateMqttPublisher.BaseTopic}/{@event.SteamId}/map";
                payload = JsonSerializer.Serialize(m.Map);
                break;
            case PlayerEvent p:
                topic = $"{GameStateMqttPublisher.BaseTopic}/{@event.SteamId}/player";
                payload = JsonSerializer.Serialize(p.Player);
                break;
            case PlayerStateEvent ps:
                topic = $"{GameStateMqttPublisher.BaseTopic}/{@event.SteamId}/player-state";
                payload = JsonSerializer.Serialize(ps.PlayerState);
                break;
            case RoundEvent r:
                topic = $"{GameStateMqttPublisher.BaseTopic}/{@event.SteamId}/round";
                payload = JsonSerializer.Serialize(r.Round);
                break;
            default: throw new SwitchExpressionException();
        }

        return new MqttApplicationMessageBuilder()
            .WithTopic(topic)
            .WithPayload(payload)
            .Build();
    }
}