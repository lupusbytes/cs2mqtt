using System.Text;
using LupusBytes.CS2.GameStateIntegration.Api.EndToEnd.Tests.Helpers;
using Microsoft.Extensions.Logging;

namespace LupusBytes.CS2.GameStateIntegration.Api.EndToEnd.Tests;

public class RequestBodyLoggingTest(DebugLoggingTestWebApplicationFactory factory)
    : IClassFixture<DebugLoggingTestWebApplicationFactory>
{
    private const string SingleLineJson = """{"provider":{"name":"Counter-Strike: Global Offensive","appid":730,"version":14020,"steamid":"76561197981496355","timestamp":1720553252}}""";
    private const string MultiLineJson = """
                                         {
                                           "provider": {
                                             "name": "Counter-Strike: Global Offensive",
                                             "appid": 730,
                                             "version": 14020,
                                             "steamid": "76561197981496355",
                                             "timestamp": 1720553252
                                           }
                                         }
                                         """;

    private readonly HttpClient httpClient = factory.CreateClient();

    [Fact]
    public async Task Logs_request_body_in_minified_format_when_media_type_is_json()
    {
        // Arrange
        using var payload = new StringContent(MultiLineJson, Encoding.UTF8, "application/json");

        // Act
        await httpClient.PostAsync("/", payload, TestContext.Current.CancellationToken);

        // Assert
        var logSnapshots = factory.LogCollector.GetSnapshot();
        logSnapshots.Should().Contain(x => x.Level == LogLevel.Debug && x.Message.Contains(SingleLineJson));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("text/plain")]
    public async Task Logs_request_body_as_is_when_media_type_is_not_json(string? mediaType)
    {
        // Arrange
        const string requestBody = "Hello, World!";
        using var payload = new StringContent(requestBody, Encoding.UTF8, mediaType);
        if (mediaType is null)
        {
            payload.Headers.ContentType = null;
        }

        // Act
        await httpClient.PostAsync("/", payload, TestContext.Current.CancellationToken);

        // Assert
        var logSnapshots = factory.LogCollector.GetSnapshot();
        logSnapshots.Should().Contain(x => x.Level == LogLevel.Debug && x.Message.Contains(requestBody));
    }
}