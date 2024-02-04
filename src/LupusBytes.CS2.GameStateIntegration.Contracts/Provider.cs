namespace LupusBytes.CS2.GameStateIntegration.Contracts;

public record Provider(
    string Name,
    int AppId,
    int Version,
    string SteamId,
    int Timestamp);