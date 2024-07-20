using LupusBytes.CS2.GameStateIntegration.Contracts;
using LupusBytes.CS2.GameStateIntegration.Events;
using LupusBytes.CS2.GameStateIntegration.Extensions;

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
            PushEvent(RoundObservers, value.ToEvent(SteamId));
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
                PushEvent(PlayerObservers, valuePlayer.ToEvent(SteamId));
            }

            if (playerState == value?.State)
            {
                return;
            }

            playerState = value?.State;
            PushEvent(PlayerStateObservers, playerState.ToEvent(SteamId));
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
            PushEvent(MapObservers, value.ToEvent(SteamId));
        }
    }

    public void ProcessEvent(GameStateData data)
    {
        Player = data.Player;
        Map = data.Map;
        Round = data.Round;
    }

    private static void PushEvent<TEvent>(IEnumerable<IObserver<TEvent>> observers, TEvent @event)
        where TEvent : BaseEvent
    {
        foreach (var observer in observers)
        {
            observer.OnNext(@event);
        }
    }
}