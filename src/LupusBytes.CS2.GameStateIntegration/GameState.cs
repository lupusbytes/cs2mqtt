using LupusBytes.CS2.GameStateIntegration.Contracts;

namespace LupusBytes.CS2.GameStateIntegration;

internal sealed class GameState(SteamId64 steamId) : ObservableGameState, IGameState
{
    private Map? map;
    private Player? player;
    private PlayerState? playerState;
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
            PushStateUpdate(RoundObservers, new StateUpdate<Round>(SteamId, round));
        }
    }

    public PlayerWithState? Player
    {
        get => player is null
            ? null
            : new PlayerWithState(player.SteamId64, player.Name, player.Team, player.Activity)
            {
                State = playerState,
            };
        private set
        {
            var valuePlayer = value is null
                ? null
                : new Player(value.SteamId64, value.Name, value.Team, value.Activity);

            if (player != valuePlayer)
            {
                player = valuePlayer;
                PushStateUpdate(PlayerObservers, new StateUpdate<Player>(SteamId, valuePlayer));
            }

            if (playerState == value?.State)
            {
                return;
            }

            playerState = value?.State;
            PushStateUpdate(PlayerStateObservers, new StateUpdate<PlayerState>(SteamId, playerState));
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
            PushStateUpdate(MapObservers, new StateUpdate<Map>(SteamId, map));
        }
    }

    public void ProcessEvent(GameStateData data)
    {
        PushStateUpdate(ProviderObservers, new StateUpdate<Provider>(SteamId, data.Provider));
        Player = data.Player;
        Map = data.Map;
        Round = data.Round;
    }
}