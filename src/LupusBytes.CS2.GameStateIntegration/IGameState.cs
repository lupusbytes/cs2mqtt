using LupusBytes.CS2.GameStateIntegration.Contracts;

namespace LupusBytes.CS2.GameStateIntegration;

public interface IGameState :
    IObservable<StateUpdate<Provider>>,
    IObservable<StateUpdate<Map>>,
    IObservable<StateUpdate<Round>>,
    IObservable<StateUpdate<Player>>,
    IObservable<StateUpdate<PlayerState>>
{
    /// <summary>
    /// The SteamID of the game state provider.
    /// </summary>
    SteamId64 SteamId { get; }
    Map? Map { get; }
    Round? Round { get; }
    PlayerWithState? Player { get; }
    void ProcessEvent(GameStateData data);
}