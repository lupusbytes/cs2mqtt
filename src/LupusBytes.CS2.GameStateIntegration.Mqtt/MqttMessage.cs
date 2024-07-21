namespace LupusBytes.CS2.GameStateIntegration.Mqtt;

public record struct MqttMessage(string Topic, string Payload, bool RetainFlag = false);