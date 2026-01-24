using System.Diagnostics.CodeAnalysis;
using System.Net.Http.Headers;
using System.Text.Json;
using Microsoft.Extensions.Logging;

namespace LupusBytes.CS2.GameStateIntegration.Mqtt.HomeAssistant;

public class SupervisorOptionsProvider(
    string supervisorToken,
    ILogger<SupervisorOptionsProvider> logger) : IMqttOptionsProvider
{
    private const string SupervisorServicesEndpoint = "http://supervisor/services/mqtt";

    public async Task<MqttOptions> GetOptionsAsync(CancellationToken cancellationToken = default)
    {
        logger.RetrievedSupervisorToken();
        try
        {
            var cfg = await GetSupervisorMqttConfigAsync(cancellationToken);
            logger.FetchedMqttInfoFromSupervisor(cfg.Host, cfg.Port);

            return new MqttOptions
            {
                Host = cfg.Host,
                Port = cfg.Port,
                UseTls = cfg.Ssl,
                Username = cfg.Username,
                Password = cfg.Password,
                ProtocolVersion = cfg.Protocol,
            };
        }
        catch (HttpRequestException ex)
        {
            logger.SupervisorApiRequestFailed(SupervisorServicesEndpoint, ex.Message);
            throw;
        }
        catch (InvalidOperationException ex)
        {
            logger.SupervisorApiRequestFailed(SupervisorServicesEndpoint, ex.Message);
            throw;
        }
    }

    [SuppressMessage("Performance", "CA1848:Use the LoggerMessage delegates", Justification = "Debug")]
    [SuppressMessage("Usage", "CA2254:Template should be a static expression", Justification = "Debug")]
    private async Task<SupervisorMqttConfig> GetSupervisorMqttConfigAsync(CancellationToken cancellationToken = default)
    {
        var httpClient = new HttpClient();
        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", supervisorToken);

        var httpResponse = await httpClient.GetAsync(new Uri(SupervisorServicesEndpoint), cancellationToken);
        var responseContent = await httpResponse.Content.ReadAsStringAsync(cancellationToken);
        logger.LogInformation("Response {ResponseContent}", responseContent);

        httpResponse.EnsureSuccessStatusCode();

        var response = JsonSerializer.Deserialize<SupervisorResponse>(responseContent);

        httpClient.Dispose();

        if (response?.Data is null)
        {
            throw new InvalidOperationException("Invalid supervisor response");
        }

        return response.Data;
    }
}