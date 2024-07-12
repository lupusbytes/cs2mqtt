using LupusBytes.CS2.GameStateIntegration.Api.Events;
using LupusBytes.CS2.GameStateIntegration.Api.Middleware;

namespace LupusBytes.CS2.GameStateIntegration.Api;

public class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        var gameStateService = new GameStateService();
        builder.Services.AddSingleton(gameStateService);
        var app = builder.Build();
        app.MapIngestionEndpoint();
        app.MapGetEndpoints();

        // Debug helpers
        app.MapIngestionDebugEndpoint();
        app.UseMiddleware<LogRequestBodyOnException>();

        app.Run();
    }
}