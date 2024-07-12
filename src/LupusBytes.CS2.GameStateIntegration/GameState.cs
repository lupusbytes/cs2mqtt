using LupusBytes.CS2.GameStateIntegration.Contracts;
using LupusBytes.CS2.GameStateIntegration.Contracts.Enums;
using LupusBytes.CS2.GameStateIntegration.Events;

namespace LupusBytes.CS2.GameStateIntegration;

internal sealed class GameState : ObservableGameState
{
    private Map? map;
    private Player? player;
    private Round? round;

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
            PushEvent(RoundObservers, CreateRoundEvent());
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
            PushEvent(PlayerObservers, CreatePlayerEvent());
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
            PushEvent(MapObservers, CreateMapEvent());
        }
    }

    internal void ProcessEvent(GameStateData @event)
    {
        if (@event.Player is not null)
        {
            Player = @event.Player;
            if (@event.Player.Activity == Activity.Menu)
            {
                Round = null;
                Map = null;
            }
        }

        if (@event.Map is not null)
        {
            Map = @event.Map;
        }

        if (@event.Round is not null)
        {
            Round = @event.Round;
        }
    }

    private MapEvent CreateMapEvent() => new(player?.SteamId64, map);
    private PlayerEvent CreatePlayerEvent() => new(player!);
    private RoundEvent CreateRoundEvent() => new(player?.SteamId64, round);

    private static void PushEvent<T>(IEnumerable<IObserver<T>> observers, T @event)
    {
        foreach (var observer in observers)
        {
            observer.OnNext(@event);
        }
    }
}