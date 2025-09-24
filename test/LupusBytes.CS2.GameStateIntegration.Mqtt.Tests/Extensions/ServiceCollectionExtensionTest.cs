using LupusBytes.CS2.GameStateIntegration.Mqtt.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace LupusBytes.CS2.GameStateIntegration.Mqtt.Tests.Extensions;

public class ServiceCollectionExtensionTest
{
    [Theory, AutoNSubstituteData]
    public void AddMqttClient_registers_MqttClient_as_Singleton_and_HostedService(
        IConfiguration configuration,
        ILogger<MqttClient> logger)
    {
        // Arrange
        var sut = new ServiceCollection();
        sut.AddSingleton(logger);

        // Act
        sut.AddMqttClient(configuration, _ => Task.CompletedTask);

        // Assert
        var serviceProvider = sut.BuildServiceProvider();

        serviceProvider
            .GetRequiredService<IMqttClient>()
            .Should()
            .BeOfType<MqttClient>();

        serviceProvider
            .GetRequiredService<IHostedService>()
            .Should()
            .BeOfType<MqttClient>();
    }
}