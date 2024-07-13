using LupusBytes.CS2.GameStateIntegration.Api.Endpoints;
using LupusBytes.CS2.GameStateIntegration.Api.Middleware;
using LupusBytes.CS2.GameStateIntegration.Mqtt;

namespace LupusBytes.CS2.GameStateIntegration.Api;

public class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        var gameStateService = new GameStateService();
        builder.Services.AddSingleton(gameStateService);

        var mqttOptions = new MqttOptions();
        builder.Configuration.GetSection(MqttOptions.Section).Bind(mqttOptions);
        builder.Services.AddHostedService(_ => new GameStateMqttPublisher(gameStateService, mqttOptions));

        var app = builder.Build();
        app.MapCS2IngestionEndpoint();
        app.MapCS2GetEndpoints();

        app.UseMiddleware<LogRequestBodyOnException>();

        app.Run();
    }
}