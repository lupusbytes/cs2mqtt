using System.Net;
using System.Text;
using LupusBytes.CS2.GameStateIntegration.Mqtt.HomeAssistant;
using Microsoft.Extensions.Logging.Abstractions;

namespace LupusBytes.CS2.GameStateIntegration.Mqtt.Tests.HomeAssistant;

public class SupervisorMqttOptionsProviderTests
{
    [Fact]
    public async Task GetOptionsAsync_returns_successfully()
    {
        // Arrange
        const string json = """
                   {
                     "data": {
                       "host": "test-broker",
                       "port": 8883,
                       "ssl": true,
                       "username": "u",
                       "password": "p",
                       "protocol": "3.1.1"
                     }
                   }
                   """;

        using var httpMessageHandler = new StubHttpMessageHandler((request, _) =>
        {
            if (request.Method == HttpMethod.Get && request.RequestUri!.AbsolutePath == "/services/mqtt")
            {
                return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent(json, Encoding.UTF8, "application/json"),
                });
            }

            Assert.Fail($"Unexpected HTTP request: {request.Method} {request.RequestUri}");
            return Task.FromResult(new HttpResponseMessage(HttpStatusCode.InternalServerError));
        });

        using var httpClient = new HttpClient(httpMessageHandler);
        httpClient.BaseAddress = new Uri("https://supervisor");

        var sut = new SupervisorMqttOptionsProvider(
            httpClient,
            NullLogger<SupervisorMqttOptionsProvider>.Instance);

        // Act
        var result = await sut.GetOptionsAsync(TestContext.Current.CancellationToken);

        // Assert
        httpMessageHandler.LastRequest.Should().NotBeNull();
        httpMessageHandler.LastRequest.Method.Should().Be(HttpMethod.Get);
        httpMessageHandler.LastRequest.RequestUri.Should().Be(new Uri("https://supervisor/services/mqtt"));
        result.Should().BeEquivalentTo(new MqttOptions
        {
            Host = "test-broker",
            Port = 8883,
            UseTls = true,
            Username = "u",
            Password = "p",
            ProtocolVersion = "3.1.1",
        });
    }

    [Theory]
    [InlineData(HttpStatusCode.BadRequest)]
    [InlineData(HttpStatusCode.Unauthorized)]
    [InlineData(HttpStatusCode.Forbidden)]
    [InlineData(HttpStatusCode.NotFound)]
    [InlineData(HttpStatusCode.InternalServerError)]
    [InlineData(HttpStatusCode.BadGateway)]
    public async Task GetOptionsAsync_throws_HttpRequestException_when_reply_is_not_success(HttpStatusCode statusCode)
    {
        // Arrange
        using var httpMessageHandler = new StubHttpMessageHandler((request, _) =>
        {
            if (request.Method == HttpMethod.Get && request.RequestUri!.AbsolutePath == "/services/mqtt")
            {
                return Task.FromResult(new HttpResponseMessage(statusCode));
            }

            Assert.Fail($"Unexpected HTTP request: {request.Method} {request.RequestUri}");
            return Task.FromResult(new HttpResponseMessage(HttpStatusCode.InternalServerError));
        });

        using var httpClient = new HttpClient(httpMessageHandler);
        httpClient.BaseAddress = new Uri("https://supervisor");

        var sut = new SupervisorMqttOptionsProvider(
            httpClient,
            NullLogger<SupervisorMqttOptionsProvider>.Instance);

        // Act
        var act = () => sut.GetOptionsAsync(TestContext.Current.CancellationToken);

        // Assert
        await act.Should().ThrowAsync<HttpRequestException>();
    }

    [Fact]
    public async Task GetOptionsAsync_throws_InvalidOperationException_when_data_is_null()
    {
        // Arrange
        using var httpMessageHandler = new StubHttpMessageHandler((request, _) =>
        {
            if (request.Method == HttpMethod.Get && request.RequestUri!.AbsolutePath == "/services/mqtt")
            {
                return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent("{}", Encoding.UTF8, "application/json"),
                });
            }

            Assert.Fail($"Unexpected HTTP request: {request.Method} {request.RequestUri}");
            return Task.FromResult(new HttpResponseMessage(HttpStatusCode.InternalServerError));
        });

        using var httpClient = new HttpClient(httpMessageHandler);
        httpClient.BaseAddress = new Uri("https://supervisor");

        var sut = new SupervisorMqttOptionsProvider(
            httpClient,
            NullLogger<SupervisorMqttOptionsProvider>.Instance);

        // Act
        var act = () => sut.GetOptionsAsync(TestContext.Current.CancellationToken);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>();
    }
}