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
        var response = await httpClient.GetAsync(MqttEndpoint, HttpCompletionOption.ResponseHeadersRead, cancellationToken);
        var content = await response.Content.ReadFromJsonAsync<SupervisorResponse<SupervisorMqttConfig>>(cancellationToken);
        if (!response.IsSuccessStatusCode)
        {
            logger.SupervisorApiRequestFailed(response.RequestMessage!.RequestUri!.ToString(), content!.Message);
            throw new HttpRequestException($"Invalid supervisor response. Message: {content!.Message}");
        }

        var config = content?.Data ?? throw new InvalidOperationException("Invalid supervisor config");
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