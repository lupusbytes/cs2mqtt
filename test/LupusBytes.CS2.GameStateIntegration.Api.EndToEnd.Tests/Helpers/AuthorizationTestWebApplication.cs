using LupusBytes.CS2.GameStateIntegration.Api.Endpoints;
using LupusBytes.CS2.GameStateIntegration.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace LupusBytes.CS2.GameStateIntegration.Api.EndToEnd.Tests.Helpers;

public sealed class AuthorizationTestWebApplication
{
    public const string ExpectedToken = nameof(ExpectedToken);

    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder();
        builder.Services.AddRouting();
        builder.Configuration.AddInMemoryCollection(
        [
            new KeyValuePair<string, string?>("GameStateOptions:Token", ExpectedToken),
        ]);
        builder.Services.AddGameStateService(builder.Configuration);

        var app = builder.Build();
        app.MapCS2IngestionEndpoint();
        app.Start();
    }
}