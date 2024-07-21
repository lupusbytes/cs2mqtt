using LupusBytes.CS2.GameStateIntegration.Contracts;
using LupusBytes.CS2.GameStateIntegration.Events;
using LupusBytes.CS2.GameStateIntegration.Extensions;

namespace LupusBytes.CS2.GameStateIntegration.Tests.Extensions;

public class EventExtensionsTest
{
    [Theory, AutoData]
    public void MapToEvent(Map map, SteamId64 steamId)
        => map.ToEvent(steamId).Should().BeEquivalentTo(new MapEvent(steamId, map));

    [Theory, AutoData]
    public void MapToEvent_when_Map_is_null(SteamId64 steamId)
        => ((Map?)null).ToEvent(steamId).Should().BeEquivalentTo(new MapEvent(steamId, Map: null));

    [Theory, AutoData]
    public void RoundToEvent(Round round, SteamId64 steamId)
        => round.ToEvent(steamId).Should().BeEquivalentTo(new RoundEvent(steamId, round));

    [Theory, AutoData]
    public void RoundToEvent_when_Round_is_null(SteamId64 steamId)
        => ((Round?)null).ToEvent(steamId).Should().BeEquivalentTo(new RoundEvent(steamId, Round: null));

    [Theory, AutoData]
    public void PlayerToEvent(Player player, SteamId64 steamId)
        => player.ToEvent(steamId).Should().BeEquivalentTo(new PlayerEvent(steamId, player));

    [Theory, AutoData]
    public void PlayerToEvent_when_PlayerState_is_null(SteamId64 steamId)
        => ((Player?)null).ToEvent(steamId).Should().BeEquivalentTo(new PlayerEvent(steamId, Player: null));

    [Theory, AutoData]
    public void PlayerStateToEvent(PlayerState playerState, SteamId64 steamId)
        => playerState.ToEvent(steamId).Should().BeEquivalentTo(new PlayerStateEvent(steamId, playerState));

    [Theory, AutoData]
    public void PlayerStateToEvent_when_PlayerState_is_null(SteamId64 steamId)
        => ((PlayerState?)null).ToEvent(steamId).Should().BeEquivalentTo(new PlayerStateEvent(steamId, PlayerState: null));
}