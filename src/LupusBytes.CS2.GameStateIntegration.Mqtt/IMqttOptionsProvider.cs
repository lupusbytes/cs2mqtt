namespace LupusBytes.CS2.GameStateIntegration.Mqtt;

public interface IMqttOptionsProvider
{
    Task<MqttOptions> GetOptionsAsync(CancellationToken cancellationToken = default);
}