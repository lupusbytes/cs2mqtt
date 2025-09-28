using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Testing;

namespace LupusBytes.CS2.GameStateIntegration.Api.EndToEnd.Tests.Helpers;

public class DebugLoggingTestWebApplicationFactory : TestWebApplicationFactory<Program>
{
    public FakeLogCollector LogCollector { get; private set; } = null!;

    protected override IHost CreateHost(IHostBuilder builder)
    {
        builder
            .ConfigureServices((context, services) =>
            {
                services.AddFakeLogging();
                context.Configuration["Logging:LogLevel:Default"] = nameof(LogLevel.Debug);
            });

        var host = base.CreateHost(builder);
        LogCollector = host.Services.GetRequiredService<FakeLogCollector>();
        return host;
    }
}