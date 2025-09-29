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
        // The SUPERVISOR_TOKEN is injected when cs2mqtt is running as an add-on under the Home Assistant Supervisor.
        // It can be used to query the Supervisor API for MQTT connection details.
        var supervisorToken = configuration["SUPERVISOR_TOKEN"];

        // To support scenarios where cs2mqtt is running as an add-on but the MQTT broker is **not**,
        // the environment variable SUPERVISOR_OVERRIDE_MQTT_CONFIG can be explicitly set to false
        // to prevent using the Supervisor for MQTT configuration.
        if (!string.IsNullOrEmpty(supervisorToken) &&
            configuration.GetValue("SUPERVISOR_OVERRIDE_MQTT_CONFIG", defaultValue: true))
        {
            // Home Assistant does **not** inject any environment variable containing the Supervisor hostname.
            // The Supervisor is however always reachable at the well-known hostname "supervisor"
            // (made available automatically via Docker networking by the Supervisor).
            // We support overriding the host for testing/development purposes using the SUPERVISOR environment variable.
            var supervisorHost = configuration.GetValue("SUPERVISOR", defaultValue: "supervisor");

            services.AddHttpClient<IMqttOptionsProvider, SupervisorMqttOptionsProvider>(client =>
            {
                client.BaseAddress = new Uri($"http://{supervisorHost}/");
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", supervisorToken);
            });
        }
        else
        {
            services.AddSingleton<IMqttOptionsProvider>(_ => new ConfigOptionsProvider(configuration));
        }

        return services;
    }
}