using LupusBytes.CS2.GameStateIntegration.Contracts;
using LupusBytes.CS2.GameStateIntegration.Events;

namespace LupusBytes.CS2.GameStateIntegration;

public interface IGameState :
    IObservable<ProviderEvent>,
    IObservable<MapEvent>,
    IObservable<RoundEvent>,
    IObservable<PlayerEvent>,
    IObservable<PlayerStateEvent>
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