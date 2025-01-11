using LupusBytes.CS2.GameStateIntegration.Contracts;

namespace LupusBytes.CS2.GameStateIntegration.Events;

public record ProviderEvent(SteamId64 SteamId, Provider? Provider) : BaseEvent(SteamId);