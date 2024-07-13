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
            PushEvent(RoundObservers, value.ToEvent(SteamId));
        }
    }

    public Player? Player
    {
        get => player;
        private set
        {
            if (player == value)
            {
                return;
            }

            player = value;
            PushEvent(PlayerObservers, value.ToEvent(SteamId));
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

    private static void PushEvent<T>(IEnumerable<IObserver<T>> observers, T @event)
    {
        foreach (var observer in observers)
        {
            observer.OnNext(@event);
        }
    }
}