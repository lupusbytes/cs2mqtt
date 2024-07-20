namespace LupusBytes.CS2.GameStateIntegration.Mqtt;

public interface IMqttClient
{
    Task PublishAsync(MqttMessage message, CancellationToken cancellationToken);
}