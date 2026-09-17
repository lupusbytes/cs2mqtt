using LupusBytes.CS2.GameStateIntegration.Contracts;
using LupusBytes.CS2.GameStateIntegration.Contracts.Enums;

namespace LupusBytes.CS2.GameStateIntegration.Benchmarks;

/// <summary>
/// Creates deterministic <see cref="GameStateData"/> payloads for the benchmarks.
/// Two payloads created with different <c>seed</c> values differ in every single block,
/// which guarantees that a state update is pushed for all six state types.
/// </summary>
internal static class GameStateDataFactory
{
    private const ulong SteamId64Base = 76561197960265728;

    public static string SteamId(int index) => (SteamId64Base + (ulong)index).ToString();

    public static GameStateData Create(string steamId, int seed) => new(
        Provider: new Provider(
            Name: "Counter-Strike: Global Offensive",
            AppId: 730,
            Version: 13983,
            SteamId64: steamId,
            Timestamp: 1700000000 + seed),
        Map: new Map(
            Mode: Mode.Competitive,
            Name: "de_dust2",
            Phase: MapPhase.Live,
            Round: seed,
            T: new TeamMapDetails(Score: seed, ConsecutiveRoundLosses: seed % 5, TimeoutsRemaining: 1, MatchesWonThisSeries: 0),
            CT: new TeamMapDetails(Score: seed + 1, ConsecutiveRoundLosses: seed % 3, TimeoutsRemaining: 1, MatchesWonThisSeries: 0)),
        Round: new Round(
            Phase: RoundPhase.Live,
            WinTeam: seed % 2 == 0 ? Team.T : Team.CT,
            Bomb: seed % 2 == 0 ? BombState.Planted : BombState.Defused),
        Player: new PlayerData(
            SteamId64: steamId,
            Name: "player" + seed,
            Team: seed % 2 == 0 ? Team.T : Team.CT,
            Activity: Activity.Playing)
        {
            State = new PlayerState(
                Health: 100 - seed,
                Armor: 100 - seed,
                Helmet: seed % 2 == 0,
                Flashed: (byte)seed,
                Smoked: (byte)seed,
                Burning: (byte)seed,
                Money: 800 + seed,
                RoundKills: seed,
                RoundKillHeadshots: seed,
                EquipmentValue: 3100 + seed),
            MatchStats = new PlayerMatchStats(
                Kills: seed,
                Assists: seed,
                Deaths: seed,
                Mvps: seed,
                Score: seed),
        });
}
