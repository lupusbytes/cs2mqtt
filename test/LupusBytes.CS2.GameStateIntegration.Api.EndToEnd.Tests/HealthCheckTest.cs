using System.Net;
using LupusBytes.CS2.GameStateIntegration.Api.EndToEnd.Tests.Helpers;

namespace LupusBytes.CS2.GameStateIntegration.Api.EndToEnd.Tests;

public class HealthCheckTest(TestWebApplicationFactory<Program> factory)
    : IClassFixture<TestWebApplicationFactory<Program>>
{
    private readonly HttpClient httpClient = factory.CreateClient();

    [Fact]
    public async Task Alive_returns_Healthy_200()
    {
        // Arrange
        factory.MqttClient.IsConnected.Returns(false);

        // Act
        var response = await httpClient.GetAsync("/alive");

        // Assert
        response.Should().HaveStatusCode(HttpStatusCode.OK);

        var body = await response.Content.ReadAsStringAsync();
        body.Should().Be("Healthy");
    }

    [Fact]
    public async Task Health_returns_Healthy_200()
    {
        // Arrange
        factory.MqttClient.IsConnected.Returns(true);

        // Act
        var response = await httpClient.GetAsync("/health");

        // Assert
        response.Should().HaveStatusCode(HttpStatusCode.OK);

        var body = await response.Content.ReadAsStringAsync();
        body.Should().Be("Healthy");
    }

    [Fact]
    public async Task Health_returns_Unhealthy_503_when_not_connected_to_Mqtt()
    {
        // Arrange
        factory.MqttClient.IsConnected.Returns(false);

        // Act
        var response = await httpClient.GetAsync("/health");

        // Assert
        response.Should().HaveStatusCode(HttpStatusCode.ServiceUnavailable);

        var body = await response.Content.ReadAsStringAsync();
        body.Should().Be("Unhealthy");
    }
}