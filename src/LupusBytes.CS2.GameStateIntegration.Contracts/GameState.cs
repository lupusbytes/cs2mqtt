namespace LupusBytes.CS2.GameStateIntegration.Contracts;

public record GameState(
    Provider? Provider,
    Map? Map,
    Round? Round,
    Player? Player);