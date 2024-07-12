using LupusBytes.CS2.GameStateIntegration.Contracts;

namespace LupusBytes.CS2.GameStateIntegration.Events;

public record PlayerEvent(Player Player) : BaseEvent(Player.SteamId64);