using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MQTTnet;
using MQTTnet.Client;

namespace LupusBytes.CS2.GameStateIntegration.Mqtt.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddMqttClient(this IServiceCollection services, IConfiguration configuration)
    {
        // Register MQTTnet.Client.IMqttClient. This instance will be injected into our own MqttClient.
        // Creating it here, instead of instantiating it inside the class, will enable us to mock/substitute it for unit tests.
        services.AddSingleton(new MqttFactory().CreateMqttClient());

        // Get options
        var mqttOptions = new MqttOptions();
        configuration.GetSection(MqttOptions.Section).Bind(mqttOptions);

        // Register concrete MqttClient class
        services.AddSingleton(sp => new MqttClient(
            sp.GetRequiredService<MQTTnet.Client.IMqttClient>(),
            mqttOptions,
            sp.GetRequiredService<ILogger<MqttClient>>()));

        // Also register the MqttClient as a hosted service, so it will connect on startup.
        services.AddHostedService<MqttClient>(s => s.GetRequiredService<MqttClient>());

        // Finally register the MqttClient as an interface, which will be injected into other services
        services.AddSingleton<IMqttClient, MqttClient>(s => s.GetRequiredService<MqttClient>());

        return services;
    }
}