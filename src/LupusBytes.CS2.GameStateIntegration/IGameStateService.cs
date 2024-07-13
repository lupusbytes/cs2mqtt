using LupusBytes.CS2.GameStateIntegration.Contracts;
using LupusBytes.CS2.GameStateIntegration.Events;

namespace LupusBytes.CS2.GameStateIntegration;

public interface IGameStateService :
    IObservable<MapEvent>,
    IObservable<PlayerEvent>,
    IObservable<RoundEvent>
{
    public Map? GetMap(SteamId64 steamId);
    public Round? GetRound(SteamId64 steamId);
    public Player? GetPlayer(SteamId64 steamId);
    void ProcessEvent(GameStateData data);
}