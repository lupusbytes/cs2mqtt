using LupusBytes.CS2.GameStateIntegration.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace LupusBytes.CS2.GameStateIntegration.Api.EndToEnd.Tests.Helpers;

public sealed class AuthorizationTestWebApplicationFactory
    : TestWebApplicationFactory<Program>
{
    public const string ExpectedToken = nameof(ExpectedToken);

    protected override IHost CreateHost(IHostBuilder builder)
    {
        builder
            .ConfigureAppConfiguration(config => config.AddInMemoryCollection(
            [
                new KeyValuePair<string, string?>("GameStateOptions:Token", ExpectedToken),
            ]))
            .ConfigureServices((context, services) => services.AddGameStateService(context.Configuration));

        return base.CreateHost(builder);
    }
}