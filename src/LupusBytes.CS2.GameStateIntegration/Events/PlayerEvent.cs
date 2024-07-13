using LupusBytes.CS2.GameStateIntegration.Contracts;

namespace LupusBytes.CS2.GameStateIntegration.Events;

public record PlayerEvent(SteamId64 SteamId64, Player? Player) : BaseEvent(SteamId64);