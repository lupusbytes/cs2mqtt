using LupusBytes.CS2.GameStateIntegration.Events;

namespace LupusBytes.CS2.GameStateIntegration;

public interface IGameStateObserver :
    IObserver<MapEvent>,
    IObserver<PlayerEvent>,
    IObserver<RoundEvent>;