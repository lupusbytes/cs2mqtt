using LupusBytes.CS2.GameStateIntegration.Contracts;

namespace LupusBytes.CS2.GameStateIntegration.Api.Events;

public record RoundEvent(SteamId64 SteamId, Round? Round) : BaseEvent(SteamId);
