using System.Net.Http.Headers;
using System.Text.Json;
using Microsoft.Extensions.Logging;

namespace LupusBytes.CS2.GameStateIntegration.Mqtt;

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
            var cfg = await GetSupervisorMqttConfig(supervisorToken, cancellationToken);
            logger.FetchedMqttInfoFromSupervisor(cfg.Host, cfg.Port);

            return new MqttOptions
            {
                Host = cfg.Host,
                Port = cfg.Port,
                UseTls = cfg.Ssl,
                Username = string.IsNullOrWhiteSpace(cfg.Username) ? string.Empty : cfg.Username,
                Password = string.IsNullOrWhiteSpace(cfg.Password) ? string.Empty : cfg.Password,
                ProtocolVersion = cfg.Protocol ?? "5.0.0",
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

    private static async Task<SupervisorMqttConfig> GetSupervisorMqttConfig(string supervisorToken, CancellationToken cancellationToken = default)
    {
        var httpClient = new HttpClient();
        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", supervisorToken);

        var httpResponse = await httpClient.GetAsync(new Uri(SupervisorServicesEndpoint), cancellationToken);
        httpResponse.EnsureSuccessStatusCode();

        await using var stream = await httpResponse.Content.ReadAsStreamAsync(cancellationToken);

        var response = await JsonSerializer.DeserializeAsync<SupervisorMqttConfigResponse>(
            stream,
            cancellationToken: cancellationToken);

        httpClient.Dispose();

        if (response?.Data is null)
        {
            throw new InvalidOperationException("Invalid supervisor response");
        }

        return response.Data;
    }
}