using LupusBytes.CS2.GameStateIntegration.Contracts;
using LupusBytes.CS2.GameStateIntegration.Events;

namespace LupusBytes.CS2.GameStateIntegration.Extensions;

public static class EventExtensions
{
    public static MapEvent ToEvent(this Map? map, SteamId64 steamId) => new(steamId, map);
    public static RoundEvent ToEvent(this Round? round, SteamId64 steamId) => new(steamId, round);
    public static PlayerEvent ToEvent(this Player? player, SteamId64 steamId) => new(steamId, player);
    public static PlayerStateEvent ToEvent(this PlayerState? playerState, SteamId64 steamId) => new(steamId, playerState);
    public static PlayerWithStateEvent ToEvent(this PlayerWithState? player, SteamId64 steamId) => new(steamId, player);
    public static PlayerEvent ToPlayerEvent(this PlayerWithState player, SteamId64 steamId) =>
        new Player(player.SteamId64, player.Name, player.Team, player.Activity).ToEvent(steamId);
}