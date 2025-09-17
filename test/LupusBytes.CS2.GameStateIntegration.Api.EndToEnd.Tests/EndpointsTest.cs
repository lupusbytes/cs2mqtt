using System.Net;
using System.Net.Http.Json;
using System.Text;
using LupusBytes.CS2.GameStateIntegration.Api.EndToEnd.Tests.Helpers;
using LupusBytes.CS2.GameStateIntegration.Contracts;
using LupusBytes.CS2.GameStateIntegration.Contracts.Enums;
using Xunit.v3.Priority;

namespace LupusBytes.CS2.GameStateIntegration.Api.EndToEnd.Tests;

[Collection("REST API")]
[TestCaseOrderer(typeof(PriorityOrderer))]
public class EndpointsTest(TestWebApplicationFactory<Program> factory)
    : IClassFixture<TestWebApplicationFactory<Program>>
{
    private readonly HttpClient httpClient = factory.CreateClient();

    [Fact, Priority(1)]
    public async Task Get_player_returns_404()
    {
        // Act
        var response = await httpClient.GetAsync("/76561197981496355/player");

        // Assert
        response.Should().HaveStatusCode(HttpStatusCode.NotFound);
    }

    [Fact, Priority(2)]
    public async Task Get_map_returns_404()
    {
        // Act
        var response = await httpClient.GetAsync("/76561197981496355/map");

        // Assert
        response.Should().HaveStatusCode(HttpStatusCode.NotFound);
    }

    [Fact, Priority(3)]
    public async Task Get_round_returns_404()
    {
        // Act
        var response = await httpClient.GetAsync("/76561197981496355/round");

        // Assert
        response.Should().HaveStatusCode(HttpStatusCode.NotFound);
    }

    [Fact, Priority(4)]
    public async Task Post_GameStateData_returns_204()
    {
        // Arrange
        const string json = """
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

    [Fact, Priority(6)]
    public async Task Get_player_returns_200_with_player()
    {
        // Act
        var response = await httpClient.GetAsync("/76561197981496355/player");

        // Assert
        response.Should().HaveStatusCode(HttpStatusCode.OK);
        var resource = await response.Content.ReadFromJsonAsync<PlayerWithState>();
        resource.Should().Be(
            new PlayerWithState(SteamId64: "247", "Bassey", Team.T, Activity.Playing)
            {
                State = new PlayerState(
                    Health: 100,
                    Armor: 100,
                    Helmet: true,
                    Flashed: 0,
                    Smoked: 0,
                    Burning: 0,
                    Money: 4750,
                    RoundKills: 0,
                    RoundKillHeadshots: 0,
                    EquipmentValue: 2900),
            });
    }

    [Fact, Priority(6)]
    public async Task Get_map_returns_200_with_map()
    {
        // Act
        var response = await httpClient.GetAsync("/76561197981496355/map");

        // Assert
        response.Should().HaveStatusCode(HttpStatusCode.OK);
        var resource = await response.Content.ReadFromJsonAsync<Map>();
        resource.Should().Be(
            new Map(
                Mode.Competitive,
                Name: "de_ancient",
                MapPhase.Live,
                Round: 8,
                T: new TeamMapDetails(6, 0, 1, 0),
                CT: new TeamMapDetails(2, 3, 1, 0)));
    }

    [Fact, Priority(7)]
    public async Task Get_round_returns_200_with_round()
    {
        // Act
        var response = await httpClient.GetAsync("/76561197981496355/round");

        // Assert
        response.Should().HaveStatusCode(HttpStatusCode.OK);
        var resource = await response.Content.ReadFromJsonAsync<Round>();
        resource.Should().Be(new Round(RoundPhase.Live, WinTeam: null, Bomb: null));
    }
}