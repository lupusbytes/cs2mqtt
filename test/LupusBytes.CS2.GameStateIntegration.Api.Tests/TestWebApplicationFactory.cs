using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace LupusBytes.CS2.GameStateIntegration.Api.Tests;

public class TestWebApplicationFactory<TProgram> : WebApplicationFactory<TProgram>
    where TProgram : class
{
    protected override IHost CreateHost(IHostBuilder builder)
    {
        builder.ConfigureHostConfiguration(config =>
        {
            // config.AddJsonFile("appsettings.Test.json", optional: false, reloadOnChange: true);
        });

        builder.ConfigureServices((_, services) =>
        {
            services.AddSingleton<IGameStateService>();
        });

        return base.CreateHost(builder);
    }
}