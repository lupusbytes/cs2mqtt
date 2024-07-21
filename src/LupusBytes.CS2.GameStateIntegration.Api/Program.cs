using LupusBytes.CS2.GameStateIntegration.Api.Endpoints;
using LupusBytes.CS2.GameStateIntegration.Api.Middleware;
using LupusBytes.CS2.GameStateIntegration.Extensions;
using LupusBytes.CS2.GameStateIntegration.Mqtt;
using LupusBytes.CS2.GameStateIntegration.Mqtt.HomeAssistant;

namespace LupusBytes.CS2.GameStateIntegration.Api;

public sealed class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        builder.Services.Configure<GameStateOptions>(builder.Configuration.GetSection(GameStateOptions.Section));
        builder.Services.AddGameStateService(builder.Configuration);

        var mqttOptions = new MqttOptions();
        builder.Configuration.GetSection(MqttOptions.Section).Bind(mqttOptions);
        builder.Services.AddSingleton<IMqttClient>(s => new MqttClient(mqttOptions, s.GetRequiredService<ILogger<MqttClient>>()));
        builder.Services.AddHostedService(s => new GameStateMqttPublisher(s.GetRequiredService<IGameStateService>(), s.GetRequiredService<IMqttClient>()));
        builder.Services.AddHostedService(s => new AvailabilityMqttPublisher(s.GetRequiredService<IGameStateService>(), s.GetRequiredService<IMqttClient>()));
        builder.Services.AddHostedService(s => new HomeAssistantDevicePublisher(s.GetRequiredService<IGameStateService>(), s.GetRequiredService<IMqttClient>()));

        var app = builder.Build();
        app.MapCS2IngestionEndpoint();
        app.MapCS2GetEndpoints();

        app.UseMiddleware<LogRequestBodyOnException>();

        app.Run();
    }
}