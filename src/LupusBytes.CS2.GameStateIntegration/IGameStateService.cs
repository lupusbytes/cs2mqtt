using LupusBytes.CS2.GameStateIntegration.Contracts;

namespace LupusBytes.CS2.GameStateIntegration;

public interface IGameStateService :
    IObservable<StateUpdate<Provider>>,
    IObservable<StateUpdate<Map>>,
    IObservable<StateUpdate<Round>>,
    IObservable<StateUpdate<Player>>,
    IObservable<StateUpdate<PlayerState>>
{
    public Map? GetMap(SteamId64 steamId);
    public Round? GetRound(SteamId64 steamId);
    public PlayerWithState? GetPlayer(SteamId64 steamId);
    void ProcessEvent(GameStateData data);
}