using LupusBytes.CS2.GameStateIntegration.Api.Middleware;

namespace LupusBytes.CS2.GameStateIntegration.Api;

public class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        builder.Services.AddSingleton(new GameState());
        var app = builder.Build();
        app.MapIngestionEndpoint();
        app.MapGetEndpoints();

        // Debug helpers
        app.MapIngestionDebugEndpoint();
        app.UseMiddleware<LogRequestBodyOnException>();

        app.Run();
    }
}