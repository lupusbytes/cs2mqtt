using System.Net;
using System.Text;
using FluentAssertions;

namespace LupusBytes.CS2.GameStateIntegration.Api.Tests;

[Collection(TestConstants.TestCollection)]
public class ListenerTest(TestWebApplicationFactory<Program> factory)
{
    private readonly HttpClient httpClient = factory.CreateClient();

    [Fact]
    public async Task PostRequestReturns204()
    {
        // Arrange
        var json = """
                      {
                        "provider": {
                          "name": "Counter-Strike: Global Offensive",
                          "appid": 730,
                          "version": 14020,
                          "steamid": "76561197981496355",
                          "timestamp": 1720553252
                        },
                        "map": {
                          "mode": "competitive",
                          "name": "de_ancient",
                          "phase": "live",
                          "round": 8,
                          "team_ct": {
                            "score": 2,
                            "consecutive_round_losses": 3,
                            "timeouts_remaining": 1,
                            "matches_won_this_series": 0
                          },
                          "team_t": {
                            "score": 6,
                            "consecutive_round_losses": 0,
                            "timeouts_remaining": 1,
                            "matches_won_this_series": 0
                          },
                          "num_matches_to_win_series": 0
                        },
                        "round": {
                          "phase": "live"
                        },
                        "player": {
                          "steamid": "247",
                          "name": "Bassey",
                          "observer_slot": 9,
                          "team": "T",
                          "activity": "playing",
                          "state": {
                            "health": 100,
                            "armor": 100,
                            "helmet": true,
                            "flashed": 0,
                            "smoked": 0,
                            "burning": 0,
                            "money": 4750,
                            "round_kills": 0,
                            "round_killhs": 0,
                            "equip_value": 2900
                          }
                        }
                      }
                      """;

        using var payload = new StringContent(json, Encoding.UTF8, "application/json");

        // Act
        var response = await httpClient.PostAsync("/", payload);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }
}