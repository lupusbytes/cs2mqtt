using LupusBytes.CS2.GameStateIntegration.Contracts;
using LupusBytes.CS2.GameStateIntegration.Events;

namespace LupusBytes.CS2.GameStateIntegration.Tests;

public class GameStateServiceTest
{
    [Theory, AutoData]
    internal void ProcessEvent_throws_ArgumentException(
        GameStateData data,
        GameStateService sut)
    {
        // Arrange
        data = data with { Provider = null };

        // Act & Assert
        sut.Invoking(x => x.ProcessEvent(data))
            .Should()
            .Throw<ArgumentException>();
    }

    [Theory, AutoData]
    internal void GetPlayer_returns_Player_by_SteamId(
        GameStateData data1,
        GameStateData data2,
        GameStateData data3,
        GameStateService sut)
    {
        // Arrange
        sut.ProcessEvent(data1);
        sut.ProcessEvent(data2);
        sut.ProcessEvent(data3);

        // Act
        var player = sut.GetPlayer(data1.Provider!.SteamId64);

        // Assert
        player.Should().BeEquivalentTo(data1.Player);
    }

    [Theory, AutoData]
    internal void GetMap_returns_Map_by_SteamId(
        GameStateData data1,
        GameStateData data2,
        GameStateData data3,
        GameStateService sut)
    {
        // Arrange
        sut.ProcessEvent(data1);
        sut.ProcessEvent(data2);
        sut.ProcessEvent(data3);

        // Act
        var map = sut.GetMap(data2.Provider!.SteamId64);

        // Assert
        map.Should().BeEquivalentTo(data2.Map);
    }

    [Theory, AutoData]
    internal void GetRound_returns_Map_by_SteamId(
        GameStateData data1,
        GameStateData data2,
        GameStateData data3,
        GameStateService sut)
    {
        // Arrange
        sut.ProcessEvent(data1);
        sut.ProcessEvent(data2);
        sut.ProcessEvent(data3);

        // Act
        var round = sut.GetRound(data3.Provider!.SteamId64);

        // Assert
        round.Should().BeEquivalentTo(data3.Round);
    }

    [Theory, AutoNSubstituteData]
    public async Task Removes_disconnected_providers_in_background(
        GameStateData data,
        IObserver<PlayerEvent> playerObserver,
        IObserver<PlayerStateEvent> playerStateObserver,
        IObserver<MapEvent> mapObserver,
        IObserver<RoundEvent> roundObserver)
    {
        // Arrange
        var options = new GameStateOptions
        {
            TimeoutInSeconds = 0.2,
            TimeoutCleanupIntervalInSeconds = 0.5,
        };

        var sut = new GameStateService(options);

        sut.Subscribe(playerObserver);
        sut.Subscribe(playerStateObserver);
        sut.Subscribe(mapObserver);
        sut.Subscribe(roundObserver);

        // Act
        sut.ProcessEvent(data);

        // Assert
        // After sending data for the SteamID to the service, it should be able return data to us for the same SteamID.
        sut.GetPlayer(data.Provider!.SteamId64).Should().NotBeNull();

        // Wait 1 second to allow the background cleanup task to perform its work.
        await Task.Delay(TimeSpan.FromSeconds(1));

        // After the wait, the background cleanup task should have removed the provider.
        sut.GetPlayer(data.Provider.SteamId64).Should().BeNull();

        // All the observers should have received null events for the corresponding SteamID.
        playerObserver.Received(1).OnNext(Arg.Is<PlayerEvent>(p =>
            p.SteamId == data.Provider.SteamId64 &&
            p.Player == null));

        playerStateObserver.Received(1).OnNext(Arg.Is<PlayerStateEvent>(ps =>
            ps.SteamId == data.Provider.SteamId64 &&
            ps.PlayerState == null));

        mapObserver.Received(1).OnNext(Arg.Is<MapEvent>(m =>
            m.SteamId == data.Provider.SteamId64 &&
            m.Map == null));

        roundObserver.Received(1).OnNext(Arg.Is<RoundEvent>(r =>
            r.SteamId == data.Provider.SteamId64 &&
            r.Round == null));
    }
}