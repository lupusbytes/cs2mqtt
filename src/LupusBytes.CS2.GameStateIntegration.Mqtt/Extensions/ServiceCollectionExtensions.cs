using System.Net.Http.Headers;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MQTTnet;
using IMqttNetClient = MQTTnet.IMqttClient;

namespace LupusBytes.CS2.GameStateIntegration.Mqtt.Extensions;

public static class ServiceCollectionExtensions
{
    private const string SupervisorServicesBaseAddress = "http://supervisor/";
    private const string SupervisorServicesEndpoint = "services/mqtt";

    public static IServiceCollection AddMqttClient(this IServiceCollection services, IConfiguration configuration)
    {
        // Register MQTTnet.Client.IMqttClient. This instance will be injected into our own MqttClient.
        // Creating it here, instead of instantiating it inside the class, will enable us to mock/substitute it for unit tests.
        services.AddSingleton(new MqttClientFactory().CreateMqttClient());

        var mqttOptions = ConfigureMqttOptions(configuration);

        // Register concrete MqttClient class
        services.AddSingleton(sp => new MqttClient(
            sp.GetRequiredService<IMqttNetClient>(),
            mqttOptions,
            sp.GetRequiredService<ILogger<MqttClient>>()));

        // Also register the MqttClient as a hosted service, so it will connect on startup.
        services.AddHostedService<MqttClient>(sp => sp.GetRequiredService<MqttClient>());

        // Finally register the MqttClient as an interface, which will be injected into other services
        services.AddSingleton<IMqttClient, MqttClient>(sp => sp.GetRequiredService<MqttClient>());

        return services;
    }

    private static MqttOptions ConfigureMqttOptions(IConfiguration configuration)
    {
        // Get options
        var mqttOptions = new MqttOptions();
        mqttOptions.ClientId = "cs2mqtt";

        var supervisorToken = Environment.GetEnvironmentVariable("SUPERVISOR_TOKEN");
        if (string.IsNullOrWhiteSpace(supervisorToken))
        {
            Console.WriteLine("No supervisor token found. Using default MQTT options.");
            configuration.GetSection(MqttOptions.Section).Bind(mqttOptions);
            return mqttOptions;
        }

        Console.WriteLine("Supervisor token found. Using supervisor MQTT options.");
        var cfg = GetSupervisorMqttConfig(supervisorToken);
        if (cfg is null)
        {
            configuration.GetSection(MqttOptions.Section).Bind(mqttOptions);
            return mqttOptions;
        }

        mqttOptions.Host = cfg.Host;
        mqttOptions.Port = cfg.Port;
        mqttOptions.UseTls = cfg.SSL;

        mqttOptions.Password = string.IsNullOrWhiteSpace(cfg.Password) ? string.Empty : cfg.Password;
        mqttOptions.Username = string.IsNullOrWhiteSpace(cfg.Username) ? string.Empty : cfg.Username;
        mqttOptions.ProtocolVersion = string.IsNullOrWhiteSpace(cfg.Protocol) ? string.Empty : cfg.Protocol;

        // Still bind remaining settings from the config file to fill missing ones
        configuration.GetSection(MqttOptions.Section).Bind(mqttOptions);

        return mqttOptions;
    }

    private static SupervisorMqttConfig? GetSupervisorMqttConfig(string supervisorToken)
    {
        using var httpClient = new HttpClient();
        httpClient.BaseAddress = new Uri(SupervisorServicesBaseAddress);
        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", supervisorToken);

        var response = httpClient.GetAsync(SupervisorServicesEndpoint).Result;
        Console.WriteLine(response.StatusCode);
        response.EnsureSuccessStatusCode();

        var json = response.Content.ReadAsStringAsync().Result;
        Console.WriteLine(json);
        var config = JsonSerializer.Deserialize<SupervisorMqttConfig>(json);

        return config;
    }
}