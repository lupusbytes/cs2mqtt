using LupusBytes.CS2.GameStateIntegration.Contracts;

namespace LupusBytes.CS2.GameStateIntegration;

/// <summary>
/// Keeps track of the game state of every connected Counter-Strike instance,
/// and raises an event whenever one of the game state blocks changes.
/// A <see cref="StateUpdateEventArgs{TState}"/> with a <see langword="null"/> state means
/// that the block is no longer available for that SteamID.
/// </summary>
public interface IGameStateService
{
    event EventHandler<StateUpdateEventArgs<Provider>>? ProviderUpdated;

    event EventHandler<StateUpdateEventArgs<Map>>? MapUpdated;

    event EventHandler<StateUpdateEventArgs<Round>>? RoundUpdated;

    event EventHandler<StateUpdateEventArgs<Player>>? PlayerUpdated;

    event EventHandler<StateUpdateEventArgs<PlayerState>>? PlayerStateUpdated;

    event EventHandler<StateUpdateEventArgs<PlayerMatchStats>>? PlayerMatchStatsUpdated;

    public Map? GetMap(SteamId64 steamId);
    public Round? GetRound(SteamId64 steamId);
    public PlayerData? GetPlayer(SteamId64 steamId);
    void ProcessEvent(GameStateData data);
}
