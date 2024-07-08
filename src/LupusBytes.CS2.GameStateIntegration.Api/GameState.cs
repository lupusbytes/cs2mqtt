using LupusBytes.CS2.GameStateIntegration.Api.Events;
using LupusBytes.CS2.GameStateIntegration.Contracts;
using LupusBytes.CS2.GameStateIntegration.Contracts.Enums;

namespace LupusBytes.CS2.GameStateIntegration.Api;

public class GameState
{
    private Map? map;
    private Player? player;
    private Provider? provider;
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
            OnRoundChanged(new RoundEventArgs(round));
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
            OnPlayerChanged(new PlayerEventArgs(player));
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
            OnMapStateChanged(new MapEventArgs(map));
        }
    }

    public Provider? Provider
    {
        get => provider;
        private set
        {
            ArgumentNullException.ThrowIfNull(value);
            if (provider == value)
            {
                return;
            }

            provider = value;
            OnProviderStateChanged(new ProviderEventArgs(value));
        }
    }

    public event EventHandler<MapEventArgs> MapEvent;
    public event EventHandler<PlayerEventArgs> PlayerEvent;
    public event EventHandler<RoundEventArgs> RoundEvent;
    public event EventHandler<ProviderEventArgs> ProviderEvent;

    protected virtual void OnMapStateChanged(MapEventArgs e) => MapEvent?.Invoke(this, e);
    protected virtual void OnPlayerChanged(PlayerEventArgs e) => PlayerEvent?.Invoke(this, e);
    protected virtual void OnRoundChanged(RoundEventArgs e) => RoundEvent?.Invoke(this, e);
    protected virtual void OnProviderStateChanged(ProviderEventArgs e) => ProviderEvent?.Invoke(this, e);

    public GameState()
    {
    }

    internal void ProcessEvent(Data @event)
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

        if (@event.Provider is not null)
        {
            Provider = @event.Provider;
        }
    }
}