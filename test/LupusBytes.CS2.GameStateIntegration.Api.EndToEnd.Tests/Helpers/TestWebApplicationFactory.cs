using LupusBytes.CS2.GameStateIntegration.Api.EndToEnd.Tests.Fakes;
using LupusBytes.CS2.GameStateIntegration.Mqtt;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;

namespace LupusBytes.CS2.GameStateIntegration.Api.EndToEnd.Tests.Helpers;

public class TestWebApplicationFactory<TProgram> : WebApplicationFactory<TProgram>
    where TProgram : class
{
    protected override IHost CreateHost(IHostBuilder builder)
    {
        builder.ConfigureHostConfiguration(config =>
        {
            config.AddJsonFile("appsettings.EndToEnd.json", optional: false, reloadOnChange: true);
        });

        builder.ConfigureServices(services =>
        {
            services.RemoveAll<IMqttClient>();
            services.AddSingleton<IMqttClient>(new FakeMqttClient());
        });

        return base.CreateHost(builder);
    }
}