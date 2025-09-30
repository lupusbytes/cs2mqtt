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
    public static IServiceCollection AddMqttClient(
        this IServiceCollection services,
        Func<IServiceProvider, Task> onFatalConnectionError)
    {
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

}