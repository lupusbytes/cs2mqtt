using System.Net.Http.Headers;
using LupusBytes.CS2.GameStateIntegration.Mqtt.HomeAssistant;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MQTTnet;
using IMqttNetClient = MQTTnet.IMqttClient;

namespace LupusBytes.CS2.GameStateIntegration.Mqtt.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddMqttClient(
        this IServiceCollection services,
        IConfiguration configuration,
        Func<IServiceProvider, Task> onFatalConnectionError)
    {
        services.AddMqttOptionsProvider(configuration);

        // Register MQTTnet.IMqttClient. This instance will be injected into our own MqttClient.
        // Creating it here, instead of instantiating it inside the class, will enable us to mock/substitute it for unit tests.
        services.AddSingleton(new MqttClientFactory().CreateMqttClient());

        // Register concrete MqttClient class
        services.AddSingleton(sp => new MqttClient(
            sp.GetRequiredService<IMqttNetClient>(),
            sp.GetRequiredService<IMqttOptionsProvider>(),
            () => onFatalConnectionError(sp),
            sp.GetRequiredService<ILogger<MqttClient>>()));

        // Also register the MqttClient as a hosted service, so it will connect on startup.
        services.AddHostedService<MqttClient>(sp => sp.GetRequiredService<MqttClient>());

        // Finally register the MqttClient as an interface, which will be injected into other services
        services.AddSingleton<IMqttClient, MqttClient>(sp => sp.GetRequiredService<MqttClient>());

        return services;
    }

    private static IServiceCollection AddMqttOptionsProvider(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // The SUPERVISOR_TOKEN and URL environment variables exists when cs2mqtt is run under the Home Assistant Supervisor.
        // The token can be used to query an internal API to fetch MQTT info and credentials.
        var supervisorToken = configuration["SUPERVISOR_TOKEN"];
        var supervisorUrl = configuration["SUPERVISOR_URL"];

        if (!string.IsNullOrEmpty(supervisorToken) &&
            !string.IsNullOrEmpty(supervisorUrl) &&
            configuration.GetValue("ZEROCONF_MQTT", defaultValue: true))
        {
            services.AddHttpClient<SupervisorOptionsProvider>(client =>
            {
                client.BaseAddress = new Uri(supervisorUrl);
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", supervisorToken);
            });
            services.AddSingleton<IMqttOptionsProvider>(sp => sp.GetRequiredService<SupervisorOptionsProvider>());
        }
        else
        {
            services.AddSingleton<IMqttOptionsProvider>(_ => new ConfigOptionsProvider(configuration));
        }

        return services;
    }
}