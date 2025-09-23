using LupusBytes.CS2.GameStateIntegration.Contracts;

namespace LupusBytes.CS2.GameStateIntegration.Events;

public record PlayerWithStateEvent(SteamId64 SteamId, PlayerWithState? Player) : BaseEvent(SteamId);