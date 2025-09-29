using LupusBytes.CS2.GameStateIntegration.Mqtt.Extensions;
using LupusBytes.CS2.GameStateIntegration.Mqtt.HomeAssistant;
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
        sut.AddMqttClient(
            configuration,
            onFatalConnectionError: _ => Task.CompletedTask);

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

    [Fact]
    public void AddMqttClient_registers_ConfigOptionsProvider()
    {
        // Arrange
        var sut = new ServiceCollection();

        // Act
        sut.AddMqttClient(
            Substitute.For<IConfiguration>(),
            onFatalConnectionError: _ => Task.CompletedTask);

        // Assert
        var serviceProvider = sut.BuildServiceProvider();

        serviceProvider
            .GetRequiredService<IMqttOptionsProvider>()
            .Should()
            .BeOfType<ConfigOptionsProvider>();
    }

    [Fact]
    public void AddMqttClient_registers_SupervisorOptionsProvider_when_SUPERVISOR_TOKEN_is_set()
    {
        // Arrange
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>(StringComparer.Ordinal)
            {
                ["SUPERVISOR_TOKEN"] = "test-token",
            })
            .Build();

        var sut = new ServiceCollection();

        // Act
        sut.AddMqttClient(
            configuration,
            onFatalConnectionError: _ => Task.CompletedTask);

        // Assert
        var serviceProvider = sut.BuildServiceProvider();

        serviceProvider
            .GetRequiredService<IMqttOptionsProvider>()
            .Should()
            .BeOfType<SupervisorMqttOptionsProvider>();
    }

    [Fact]
    public void AddMqttClient_registers_ConfigOptionsProvider_when_SUPERVISOR_OVERRIDE_MQTT_CONFIG_is_false()
    {
        // Arrange
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>(StringComparer.Ordinal)
            {
                ["SUPERVISOR_TOKEN"] = "test-token",
                ["SUPERVISOR_OVERRIDE_MQTT_CONFIG"] = "false",
            })
            .Build();

        var sut = new ServiceCollection();

        // Act
        sut.AddMqttClient(
            configuration,
            onFatalConnectionError: _ => Task.CompletedTask);

        // Assert
        var serviceProvider = sut.BuildServiceProvider();

        serviceProvider
            .GetRequiredService<IMqttOptionsProvider>()
            .Should()
            .BeOfType<ConfigOptionsProvider>();
    }
}