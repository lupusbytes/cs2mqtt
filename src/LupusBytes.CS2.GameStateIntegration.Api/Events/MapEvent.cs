using LupusBytes.CS2.GameStateIntegration.Contracts;

namespace LupusBytes.CS2.GameStateIntegration.Api.Events;

public record MapEvent(SteamId64 SteamId, Map? Map) : BaseEvent(SteamId);