using Microsoft.Extensions.Configuration;

namespace LupusBytes.CS2.GameStateIntegration.Mqtt;

public class ConfigOptionsProvider(IConfiguration configuration) : IMqttOptionsProvider
{
    public Task<MqttOptions> GetOptionsAsync(CancellationToken cancellationToken = default)
    {
        var mqttOptions = new MqttOptions();
        configuration.GetSection(MqttOptions.Section).Bind(mqttOptions);
        return Task.FromResult(mqttOptions);
    }
}