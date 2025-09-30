using LupusBytes.CS2.GameStateIntegration.Api.Endpoints;
using LupusBytes.CS2.GameStateIntegration.Api.Extensions;
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
        builder.Services.AddGameStateService(builder.Configuration);
        builder.Services.AddMqttClient(builder.Configuration, onFatalConnectionError: sp => sp.GetStopApplicationTask());
        builder.Services.AddHostedService<GameStateMqttPublisher>();
        builder.Services.AddHostedService<AvailabilityMqttPublisher>();
        builder.Services.AddHostedService<HomeAssistantDevicePublisher>();
        builder.Services.ConfigureHealthChecks();

        var app = builder.Build();
        app.MapCS2IngestionEndpoint();
        app.MapCS2GetEndpoints();
        app.MapHealthCheckEndpoints();

        if (app.Logger.IsEnabled(LogLevel.Debug))
        {
            app.UseMiddleware<RequestBodyLogger>();
        }

        app.Run();
    }
}