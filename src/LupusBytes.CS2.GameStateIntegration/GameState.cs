using LupusBytes.CS2.GameStateIntegration.Contracts;
using LupusBytes.CS2.GameStateIntegration.Contracts.Enums;
using LupusBytes.CS2.GameStateIntegration.Extensions;

namespace LupusBytes.CS2.GameStateIntegration;

internal sealed class GameState(SteamId64 steamId) : ObservableGameState, IGameState
{
    private Map? map;
    private Player? player;
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
            PushEvent(RoundObservers, round.ToEvent(player?.SteamId64));
        }
    }

    public Player? Player
    {
        get => player;
        private set
        {
            ArgumentNullException.ThrowIfNull(value);
            if (player == value)
            {
                return;
            }

            player = value;
            PushEvent(PlayerObservers, value.ToEvent());
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
            PushEvent(MapObservers, value.ToEvent(player?.SteamId64));
        }
    }

    public void ProcessEvent(GameStateData data)
    {
        if (data.Player is not null)
        {
            Player = data.Player;
            if (data.Player.Activity == Activity.Menu)
            {
                Round = null;
                Map = null;
            }
        }

        if (data.Map is not null)
        {
            Map = data.Map;
        }

        if (data.Round is not null)
        {
            Round = data.Round;
        }
    }

    private static void PushEvent<T>(IEnumerable<IObserver<T>> observers, T @event)
    {
        foreach (var observer in observers)
        {
            observer.OnNext(@event);
        }
    }
}