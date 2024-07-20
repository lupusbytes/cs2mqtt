namespace LupusBytes.CS2.GameStateIntegration.Mqtt;

public class MqttMessage
{
    public string Topic { get; set; } = string.Empty;

    public string Payload { get; set; } = string.Empty;
}