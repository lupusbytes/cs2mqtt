using System.Diagnostics.CodeAnalysis;
using LupusBytes.CS2.GameStateIntegration.Contracts;
using LupusBytes.CS2.GameStateIntegration.Contracts.Enums;
using LupusBytes.CS2.GameStateIntegration.Mqtt.Extensions;

namespace LupusBytes.CS2.GameStateIntegration.Mqtt.Tests.Extensions;

[SuppressMessage("Usage", "xUnit1045:Avoid using TheoryData type arguments that might not be serializable", Justification = "No serialization problems")]
public class StateUpdateExtensionsTest
{
    private static readonly SteamId64 SteamId = SteamId64.FromString("76561197981496355");

    [Theory]
    [MemberData(nameof(MapCases))]
    public void ToMqttMessage_Map(
        string expectedTopic,
        string expectedPayload,
        StateUpdate<Map> mapUpdate)
        => Assert(mapUpdate.ToMqttMessage(), expectedTopic, expectedPayload);

    [Theory]
    [MemberData(nameof(RoundCases))]
    public void ToMqttMessage_Round(
        string expectedTopic,
        string expectedPayload,
        StateUpdate<Round> roundUpdate)
        => Assert(roundUpdate.ToMqttMessage(), expectedTopic, expectedPayload);

    [Theory]
    [MemberData(nameof(PlayerCases))]
    public void ToMqttMessage_Player(
        string expectedTopic,
        string expectedPayload,
        StateUpdate<Player> playerUpdate)
        => Assert(playerUpdate.ToMqttMessage(), expectedTopic, expectedPayload);

    [Theory]
    [MemberData(nameof(PlayerStateCases))]
    public void ToMqttMessage_PlayerState(
        string expectedTopic,
        string expectedPayload,
        StateUpdate<PlayerState> playerStateUpdate)
        => Assert(playerStateUpdate.ToMqttMessage(), expectedTopic, expectedPayload);

    public static TheoryData<string, string, StateUpdate<Player>> PlayerCases => new()
    {
        {
            $"{MqttConstants.BaseTopic}/{SteamId}/player",
            string.Empty,
            new StateUpdate<Player>(SteamId, State: null)
        },
        {
            $"{MqttConstants.BaseTopic}/{SteamId}/player",
            """{"steamid":"76561197981496355","name":"lupus","team":"T","activity":"Menu"}""",
            new StateUpdate<Player>(SteamId, new Player(
                SteamId.ToString(),
                "lupus",
                Team.T,
                Activity.Menu))
        },
    };

    public static TheoryData<string, string, StateUpdate<PlayerState>> PlayerStateCases => new()
    {
        {
            $"{MqttConstants.BaseTopic}/{SteamId}/player-state",
            string.Empty,
            new StateUpdate<PlayerState>(SteamId, State: null)
        },
        {
            $"{MqttConstants.BaseTopic}/{SteamId}/player-state",
            """{"health":100,"armor":53,"helmet":true,"flashed":0,"smoked":255,"burning":0,"money":16000,"round_kills":1,"round_killhs":1,"equip_value":5350}""",
            new StateUpdate<PlayerState>(SteamId, new PlayerState(
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
    };

    public static TheoryData<string, string, StateUpdate<Map>> MapCases => new()
    {
        {
            $"{MqttConstants.BaseTopic}/{SteamId}/map",
            string.Empty,
            new StateUpdate<Map>(SteamId, State: null)
        },
        {
            $"{MqttConstants.BaseTopic}/{SteamId}/map",
            """{"mode":"Casual","name":"fy_iceworld","phase":"Live","round":44,"team_t":{"score":23,"consecutive_round_losses":0,"timeouts_remaining":0,"matches_won_this_series":0},"team_ct":{"score":20,"consecutive_round_losses":3,"timeouts_remaining":0,"matches_won_this_series":0}}""",
            new StateUpdate<Map>(SteamId, new Map(
                Mode.Casual,
                "fy_iceworld",
                MapPhase.Live,
                44,
                new TeamMapDetails(23, 0, 0, 0),
                new TeamMapDetails(20, 3, 0, 0)))
        },
    };

    public static TheoryData<string, string, StateUpdate<Round>> RoundCases => new()
    {
        {
            $"{MqttConstants.BaseTopic}/{SteamId}/round",
            string.Empty,
            new StateUpdate<Round>(SteamId, State: null)
        },
        {
            $"{MqttConstants.BaseTopic}/{SteamId}/round",
            """{"phase":"Over","win_team":"CT","bomb":"Defused"}""",
            new StateUpdate<Round>(SteamId, new Round(
                RoundPhase.Over,
                WinTeam: Team.CT,
                BombState.Defused))
        },
    };

    private static void Assert(
        MqttMessage actual,
        string expectedTopic,
        string expectedPayload)
    {
        actual.Topic.Should().Be(expectedTopic);
        actual.Payload.Should().Be(expectedPayload);
        actual.RetainFlag.Should().BeTrue();
    }
}