using System.Net.Http.Json;
using Microsoft.Extensions.Logging;

namespace LupusBytes.CS2.GameStateIntegration.Mqtt.HomeAssistant;

public class SupervisorOptionsProvider(
    HttpClient httpClient,
    ILogger<SupervisorOptionsProvider> logger) : IMqttOptionsProvider
{
    private static readonly Uri MqttEndpoint = new("services/mqtt", UriKind.Relative);

    public async Task<MqttOptions> GetOptionsAsync(CancellationToken cancellationToken = default)
    {
        logger.RetrievedSupervisorToken();
        var response = await httpClient.GetAsync(MqttEndpoint, cancellationToken);
        if (!response.IsSuccessStatusCode)
        {
            throw new HttpRequestException(
                $"Supervisor returned status code {response.StatusCode} " +
                $"with message: {response.Content.ReadAsStringAsync(cancellationToken)}");
        }

        var responseContent = await response.Content.ReadFromJsonAsync<SupervisorResponse<SupervisorMqttConfig>>(cancellationToken);
        var config = responseContent?.Data ?? throw new InvalidOperationException("Supervisor MQTT config is null");

        logger.FetchedMqttInfoFromSupervisor(config.Host, config.Port);
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