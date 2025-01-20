namespace LupusBytes.CS2.GameStateIntegration.Mqtt;

public interface IMqttClient
{
    bool IsConnected { get; }

    Task PublishAsync(MqttMessage message, CancellationToken cancellationToken);
}