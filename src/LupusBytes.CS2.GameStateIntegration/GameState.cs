using LupusBytes.CS2.GameStateIntegration.Contracts;

namespace LupusBytes.CS2.GameStateIntegration;

internal sealed class GameState(
    SteamId64 steamId,
    bool ignoreSpectatedPlayers,
    IGameStateUpdateListener listener) : IGameState
{
    private Map? map;
    private Player? player;
    private PlayerState? playerState;
    private PlayerMatchStats? playerMatchStats;
    private Round? round;

    public SteamId64 SteamId => steamId;

    public Round? Round
    {
        get => round;
        private set
        {
            if (round == value)
            {
                return;
            }

            round = value;
            listener.OnRoundUpdated(new StateUpdateEventArgs<Round>(SteamId, round));
        }
    }

    public PlayerData? Player
    {
        get => player is null
            ? null
            : new PlayerData(player.SteamId64, player.Name, player.Team, player.Activity)
            {
                State = playerState,
                MatchStats = playerMatchStats,
            };
        private set
        {
            if (ignoreSpectatedPlayers && value?.SteamId64 != steamId)
            {
                return;
            }

            var valuePlayer = value is null
                ? null
                : new Player(value.SteamId64, value.Name, value.Team, value.Activity);

            if (player != valuePlayer)
            {
                player = valuePlayer;
                listener.OnPlayerUpdated(new StateUpdateEventArgs<Player>(SteamId, valuePlayer));
            }

            if (playerState != value?.State)
            {
                playerState = value?.State;
                listener.OnPlayerStateUpdated(new StateUpdateEventArgs<PlayerState>(SteamId, playerState));
            }

            if (playerMatchStats == value?.MatchStats)
            {
                return;
            }

            playerMatchStats = value?.MatchStats;
            listener.OnPlayerMatchStatsUpdated(new StateUpdateEventArgs<PlayerMatchStats>(SteamId, playerMatchStats));
        }
    }

    public Map? Map
    {
        get => map;
        private set
        {
            if (map == value)
            {
                return;
            }

            map = value;
            listener.OnMapUpdated(new StateUpdateEventArgs<Map>(SteamId, map));
        }
    }

    public void ProcessEvent(GameStateData data)
    {
        listener.OnProviderUpdated(new StateUpdateEventArgs<Provider>(SteamId, data.Provider));
        Player = data.Player;
        Map = data.Map;
        Round = data.Round;
    }
}
