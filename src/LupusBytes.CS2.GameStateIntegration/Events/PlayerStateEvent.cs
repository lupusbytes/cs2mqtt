using LupusBytes.CS2.GameStateIntegration.Contracts;

namespace LupusBytes.CS2.GameStateIntegration.Events;

public record PlayerStateEvent(SteamId64 SteamId, PlayerState? PlayerState) : BaseEvent(SteamId);