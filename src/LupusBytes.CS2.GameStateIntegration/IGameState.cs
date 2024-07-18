using LupusBytes.CS2.GameStateIntegration.Contracts;
using LupusBytes.CS2.GameStateIntegration.Events;

namespace LupusBytes.CS2.GameStateIntegration;

public interface IGameState :
    IObservable<MapEvent>,
    IObservable<RoundEvent>,
    IObservable<PlayerEvent>
{
    /// <summary>
    /// The SteamID of the game state provider.
    /// </summary>
    SteamId64 SteamId { get; }
    Map? Map { get; }
    Round? Round { get; }
    Player? Player { get; }
    void ProcessEvent(GameStateData data);
}