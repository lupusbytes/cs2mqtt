using LupusBytes.CS2.GameStateIntegration.Contracts;
using LupusBytes.CS2.GameStateIntegration.Contracts.Enums;
using LupusBytes.CS2.GameStateIntegration.Events;
using LupusBytes.CS2.GameStateIntegration.Mqtt.Extensions;

namespace LupusBytes.CS2.GameStateIntegration.Mqtt.Tests.Extensions;

public class EventExtensionsTest
{
    private static readonly SteamId64 SteamId = SteamId64.FromString("76561197981496355");

    [Theory]
    [MemberData(nameof(TestCases))]
    public void ToMqttMessage(
        string expectedTopic,
        string expectedPayload,
        BaseEvent @event)
    {
        // Act
        var result = @event.ToMqttMessage();

        // Assert
        result.Topic.Should().Be(expectedTopic);
        result.Payload.Should().Be(expectedPayload);
        result.RetainFlag.Should().BeTrue();
    }

    public static readonly TheoryData<string, string, BaseEvent> TestCases = new()
    {
        {
            $"{MqttConstants.BaseTopic}/{SteamId}/player",
            string.Empty,
            new PlayerEvent(SteamId, Player: null)
        },
        {
            $"{MqttConstants.BaseTopic}/{SteamId}/player-state",
            string.Empty,
            new PlayerStateEvent(SteamId, PlayerState: null)
        },
        {
            $"{MqttConstants.BaseTopic}/{SteamId}/map",
            string.Empty,
            new MapEvent(SteamId, Map: null)
        },
        {
            $"{MqttConstants.BaseTopic}/{SteamId}/round",
            string.Empty,
            new RoundEvent(SteamId, Round: null)
        },
        {
            $"{MqttConstants.BaseTopic}/{SteamId}/player",
            """{"steamid":"76561197981496355","name":"lupus","team":"T","activity":"Menu"}""",
            new PlayerEvent(SteamId, new Player(
                SteamId.ToString(),
                "lupus",
                Team.T,
                Activity.Menu))
        },
        {
            $"{MqttConstants.BaseTopic}/{SteamId}/player-state",
            """{"health":100,"armor":53,"helmet":true,"flashed":0,"smoked":255,"burning":0,"money":16000,"round_kills":1,"round_killhs":1,"equip_value":5350}""",
            new PlayerStateEvent(SteamId, new PlayerState(
                Health: 100,
                Armor: 53,
                Helmet: true,
                Flashed: 0,
                Smoked: 255,
                Burning: 0,
                Money: 16000,
                RoundKills: 1,
                RoundKillHeadshots: 1,
                EquipmentValue: 5350))
        },
        {
            $"{MqttConstants.BaseTopic}/{SteamId}/map",
            """{"mode":"Casual","name":"fy_iceworld","phase":"Live","round":44,"team_t":{"score":23,"consecutive_round_losses":0,"timeouts_remaining":0,"matches_won_this_series":0},"team_ct":{"score":20,"consecutive_round_losses":3,"timeouts_remaining":0,"matches_won_this_series":0}}""",
            new MapEvent(SteamId, new Map(
                Mode.Casual,
                "fy_iceworld",
                MapPhase.Live,
                44,
                new TeamMapDetails(23, 0, 0, 0),
                new TeamMapDetails(20, 3, 0, 0)))
        },
        {
            $"{MqttConstants.BaseTopic}/{SteamId}/round",
            """{"phase":"Over","win_team":"CT","bomb":"Defused"}""",
            new RoundEvent(SteamId, new Round(
                RoundPhase.Over,
                WinTeam: Team.CT,
                BombState.Defused))
        },
    };
}