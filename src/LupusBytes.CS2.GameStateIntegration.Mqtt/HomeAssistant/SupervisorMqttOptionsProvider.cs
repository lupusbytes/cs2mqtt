using System.Net.Http.Json;
using Microsoft.Extensions.Logging;

namespace LupusBytes.CS2.GameStateIntegration.Mqtt.HomeAssistant;

public class SupervisorMqttOptionsProvider(
    HttpClient httpClient,
    ILogger<SupervisorMqttOptionsProvider> logger) : IMqttOptionsProvider
{
    private static readonly Uri MqttEndpoint = new("services/mqtt", UriKind.Relative);

    public async Task<MqttOptions> GetOptionsAsync(CancellationToken cancellationToken = default)
    {
        logger.RequestingSupervisorMqttConfig();

        using var response = await httpClient.GetAsync(MqttEndpoint, cancellationToken);
        if (!response.IsSuccessStatusCode)
        {
            var message = await response.Content.ReadAsStringAsync(cancellationToken);
            throw new HttpRequestException($"Home Assistant Supervisor returned status code {response.StatusCode} with message '{message}'");
        }

        var responseContent = await response.Content.ReadFromJsonAsync<SupervisorResponse<SupervisorMqttConfig>>(cancellationToken);
        var config = responseContent!.Data ?? throw new InvalidOperationException("Supervisor MQTT config is null");

        logger.RetrievedSupervisorMqttConfig(config.Addon);

        return new MqttOptions
        {
            Host = config.Host,
            Port = config.Port,
            UseTls = config.Ssl,
            Username = config.Username,
            Password = config.Password,
            ProtocolVersion = config.Protocol,
        };
    }
}