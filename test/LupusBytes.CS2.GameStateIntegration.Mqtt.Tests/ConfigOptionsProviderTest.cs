using Microsoft.Extensions.Configuration;

namespace LupusBytes.CS2.GameStateIntegration.Mqtt.Tests;

public class ConfigOptionsProviderTest
{
    [Fact]
    public async Task GetOptionsAsync_binds_configuration_from_Mqtt_section()
    {
        // Arrange
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>(StringComparer.Ordinal)
            {
                ["Mqtt:Host"] = "mqtt.example.com",
                ["Mqtt:Port"] = "8883",
                ["Mqtt:ProtocolVersion"] = "3.1.1",
                ["Mqtt:UseTls"] = "true",
                ["Mqtt:Username"] = "test-user",
                ["Mqtt:Password"] = "test-pass",
                ["Mqtt:ClientId"] = "test-client",
            })
            .Build();

        var sut = new ConfigOptionsProvider(configuration);

        // Act
        var result = await sut.GetOptionsAsync(TestContext.Current.CancellationToken);

        // Assert
        result.Should().BeEquivalentTo(new MqttOptions
        {
            Host = "mqtt.example.com",
            Port = 8883,
            ProtocolVersion = "3.1.1",
            UseTls = true,
            Username = "test-user",
            Password = "test-pass",
            ClientId = "test-client",
        });
    }
}