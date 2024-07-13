using LupusBytes.CS2.GameStateIntegration.Api.Endpoints;
using LupusBytes.CS2.GameStateIntegration.Api.Middleware;
using LupusBytes.CS2.GameStateIntegration.Extensions;
using LupusBytes.CS2.GameStateIntegration.Mqtt;

namespace LupusBytes.CS2.GameStateIntegration.Api;

public sealed class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        builder.Services.AddGameStateService();

        var mqttOptions = new MqttOptions();
        builder.Configuration.GetSection(MqttOptions.Section).Bind(mqttOptions);
        builder.Services.AddHostedService(b => new GameStateMqttPublisher(b.GetRequiredService<IGameStateService>(), mqttOptions));

        var app = builder.Build();
        app.MapCS2IngestionEndpoint();
        app.MapCS2GetEndpoints();

        app.UseMiddleware<LogRequestBodyOnException>();

        app.Run();
    }
}