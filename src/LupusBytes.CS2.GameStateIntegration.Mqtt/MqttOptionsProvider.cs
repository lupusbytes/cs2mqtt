using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace LupusBytes.CS2.GameStateIntegration.Mqtt;

public class MqttOptionsProvider : IMqttOptionsProvider
{
    private const string SupervisorServicesEndpoint = "http://supervisor/services/mqtt";

    private readonly HttpClient httpClient;
    private readonly ILogger<MqttOptionsProvider> logger;
    private MqttOptions mqttOptions;
    private bool fetched;

    public MqttOptionsProvider(ILogger<MqttOptionsProvider> logger, HttpClient httpClient)
    {
        this.httpClient = httpClient;
        this.logger = logger;
        mqttOptions = new MqttOptions();
        fetched = false;
    }

    public async Task<MqttOptions> GetOptionsAsync(CancellationToken cancellationToken = default)
    {
        // TODO: singleton checking whether config has been filled?
        if (fetched)
        {
            return mqttOptions;
        }

        fetched = true;
        mqttOptions.ClientId = "cs2mqtt";

        var supervisorToken = Environment.GetEnvironmentVariable("SUPERVISOR_TOKEN");
        if (string.IsNullOrWhiteSpace(supervisorToken))
        {
            // FIXME empty config
            logger.NoSupervisorTokenFound();
            fetched = true;
            return mqttOptions;
        }

        logger.RetrievedSupervisorToken();
        try
        {
            var cfg = await GetSupervisorMqttConfig(supervisorToken, cancellationToken);

            logger.FetchedMqttInfoFromSupervisor(cfg.Host, cfg.Port);

            mqttOptions.Host = cfg.Host;
            mqttOptions.Port = cfg.Port;
            mqttOptions.UseTls = cfg.SSL;
            mqttOptions.Username = string.IsNullOrWhiteSpace(cfg.Username) ? string.Empty : cfg.Username;
            mqttOptions.Password = string.IsNullOrWhiteSpace(cfg.Password) ? string.Empty : cfg.Password;
            mqttOptions.ProtocolVersion = string.IsNullOrWhiteSpace(cfg.Protocol) ? string.Empty : cfg.Protocol;

            fetched = true;
        }
        catch (Exception ex)
        {
            logger.SupervisorApiRequestFailed(SupervisorServicesEndpoint, ex.Message);
            return mqttOptions;
        }

        return mqttOptions;
    }


    private async Task<SupervisorMqttConfig> GetSupervisorMqttConfig(string supervisorToken, CancellationToken cancellationToken = default)
    {
        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", supervisorToken);

        var httpResponse = await httpClient.GetAsync(new Uri(SupervisorServicesEndpoint), cancellationToken);
        httpResponse.EnsureSuccessStatusCode();

        await using var stream = await httpResponse.Content.ReadAsStreamAsync(cancellationToken);

        var response = await JsonSerializer.DeserializeAsync<SupervisorMqttConfigResponse>(
            stream,
            cancellationToken: cancellationToken
        );

        var contentString = await httpResponse.Content.ReadAsStringAsync(cancellationToken);
        logger.LogInformation("Supervisor response: {Response}", contentString);


        if (response?.Data is null)
        {
            throw new InvalidOperationException("Invalid supervisor response");
        }

        return response.Data;
    }
}