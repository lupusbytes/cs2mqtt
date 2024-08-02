using LupusBytes.CS2.GameStateIntegration.Api.Endpoints;
using LupusBytes.CS2.GameStateIntegration.Api.Middleware;
using LupusBytes.CS2.GameStateIntegration.Extensions;
using LupusBytes.CS2.GameStateIntegration.Mqtt;
using LupusBytes.CS2.GameStateIntegration.Mqtt.Extensions;
using LupusBytes.CS2.GameStateIntegration.Mqtt.HomeAssistant;

namespace LupusBytes.CS2.GameStateIntegration.Api;

public sealed class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        builder.Services.Configure<GameStateOptions>(builder.Configuration.GetSection(GameStateOptions.Section));
        builder.Services.AddGameStateService(builder.Configuration);
        builder.Services.AddMqttClient(builder.Configuration);
        builder.Services.AddHostedService<GameStateMqttPublisher>();
        builder.Services.AddHostedService<AvailabilityMqttPublisher>();
        builder.Services.AddHostedService<HomeAssistantDevicePublisher>();

        var app = builder.Build();
        app.MapCS2IngestionEndpoint();
        app.MapCS2GetEndpoints();

        app.UseMiddleware<LogRequestBodyOnException>();

        app.Run();
    }
}