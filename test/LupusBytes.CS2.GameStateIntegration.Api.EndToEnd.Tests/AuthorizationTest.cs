using System.Net;
using System.Text;
using LupusBytes.CS2.GameStateIntegration.Api.EndToEnd.Tests.Helpers;

namespace LupusBytes.CS2.GameStateIntegration.Api.EndToEnd.Tests;

public class AuthorizationTest(TestWebApplicationFactory<AuthorizationTestWebApplication> factory)
    : IClassFixture<TestWebApplicationFactory<AuthorizationTestWebApplication>>
{
    private const string ProviderJsonObject =
        """
        {
          "name": "Counter-Strike: Global Offensive",
          "appid": 730,
          "version": 14020,
          "steamid": "76561197981496355",
          "timestamp": 1720553252
        }
        """;

    private readonly HttpClient httpClient = factory.CreateClient();

    [Fact]
    public async Task Post_with_valid_token_returns_successful()
    {
        // Arrange
        const string json = $$"""
                                {
                                  "provider": {{ProviderJsonObject}},
                                  "auth": {
                                    "token": "{{AuthorizationTestWebApplication.ExpectedToken}}"
                                  }
                                }
                              """;

        using var payload = new StringContent(json, Encoding.UTF8, "application/json");

        // Act
        var response = await httpClient.PostAsync("/", payload);

        // Assert
        response.Should().BeSuccessful();
    }

    [Fact]
    public async Task Post_with_invalid_token_returns_401()
    {
        // Arrange
        const string json = $$"""
                                {
                                  "provider": {{ProviderJsonObject}},
                                  "auth": {
                                    "token": "InvalidToken"
                                  }
                                }
                              """;

        using var payload = new StringContent(json, Encoding.UTF8, "application/json");

        // Act
        var response = await httpClient.PostAsync("/", payload);

        // Assert
        response.Should().HaveStatusCode(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Post_with_no_token_returns_401()
    {
        // Arrange
        const string json = $$"""{"provider":{{ProviderJsonObject}}}""";

        using var payload = new StringContent(json, Encoding.UTF8, "application/json");

        // Act
        var response = await httpClient.PostAsync("/", payload);

        // Assert
        response.Should().HaveStatusCode(HttpStatusCode.Unauthorized);
    }
}