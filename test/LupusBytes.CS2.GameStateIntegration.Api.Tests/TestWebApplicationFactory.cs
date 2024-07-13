using LupusBytes.CS2.GameStateIntegration.Mqtt;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace LupusBytes.CS2.GameStateIntegration.Api.Tests;

public class TestWebApplicationFactory<TProgram> : WebApplicationFactory<TProgram>
    where TProgram : class
{
    protected override IHost CreateHost(IHostBuilder builder)
    {
        builder.ConfigureHostConfiguration(config =>
        {
            config.AddJsonFile("appsettings.Test.json", optional: false, reloadOnChange: true);
        });

        builder.ConfigureServices(services =>
        {
            var descriptor = services.SingleOrDefault(s =>
                s.ServiceType == typeof(IHostedService) &&
                s.ImplementationFactory?.Method.ReturnType == typeof(GameStateMqttPublisher));

            if (descriptor is not null)
            {
                services.Remove(descriptor);
            }
        });

        return base.CreateHost(builder);
    }
}