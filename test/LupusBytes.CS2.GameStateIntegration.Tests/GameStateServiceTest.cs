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

    [Theory, AutoNSubstituteData]
    internal void ProcessEvent_sends_events_from_multiple_providers(
        Provider provider1,
        Provider provider2,
        GameStateData data1,
        GameStateData data2,
        GameStateData data3,
        GameStateData data4,
        IObserver<PlayerEvent> playerObserver,
        IObserver<PlayerStateEvent> playerStateObserver,
        IObserver<MapEvent> mapObserver,
        IObserver<RoundEvent> roundObserver,
        GameStateService sut)
    {
        // Arrange
        sut.Subscribe(playerObserver);
        sut.Subscribe(playerStateObserver);
        sut.Subscribe(mapObserver);
        sut.Subscribe(roundObserver);

        var data = new List<GameStateData>
        {
            data1 with { Provider = provider1 },
            data2 with { Provider = provider2 },
            data3 with { Provider = provider1 },
            data4 with { Provider = provider2 },
        };

        // Act
        foreach (var @event in data)
        {
            sut.ProcessEvent(@event);
        }

        // Assert
        // Should have received 2 events from provider1
        playerObserver.Received(2).OnNext(Arg.Is<PlayerEvent>(r => r.SteamId == provider1.SteamId64));
        playerStateObserver.Received(2).OnNext(Arg.Is<PlayerStateEvent>(r => r.SteamId == provider1.SteamId64));
        mapObserver.Received(2).OnNext(Arg.Is<MapEvent>(r => r.SteamId == provider1.SteamId64));
        roundObserver.Received(2).OnNext(Arg.Is<RoundEvent>(r => r.SteamId == provider1.SteamId64));

        // Should have received 2 events from provider2
        playerObserver.Received(2).OnNext(Arg.Is<PlayerEvent>(r => r.SteamId == provider2.SteamId64));
        playerStateObserver.Received(2).OnNext(Arg.Is<PlayerStateEvent>(r => r.SteamId == provider2.SteamId64));
        mapObserver.Received(2).OnNext(Arg.Is<MapEvent>(r => r.SteamId == provider2.SteamId64));
        roundObserver.Received(2).OnNext(Arg.Is<RoundEvent>(r => r.SteamId == provider2.SteamId64));
    }

    [Theory, AutoNSubstituteData]
    internal void ProcessEvent_sends_events_to_multiple_observers(
        GameStateData data,
        IObserver<PlayerEvent> playerObserver1,
        IObserver<PlayerStateEvent> playerStateObserver1,
        IObserver<MapEvent> mapObserver1,
        IObserver<RoundEvent> roundObserver1,
        IObserver<PlayerEvent> playerObserver2,
        IObserver<PlayerStateEvent> playerStateObserver2,
        IObserver<MapEvent> mapObserver2,
        IObserver<RoundEvent> roundObserver2,
        GameStateService sut)
    {
        // Arrange
        sut.Subscribe(playerObserver1);
        sut.Subscribe(playerStateObserver1);
        sut.Subscribe(mapObserver1);
        sut.Subscribe(roundObserver1);

        sut.Subscribe(playerObserver2);
        sut.Subscribe(playerStateObserver2);
        sut.Subscribe(mapObserver2);
        sut.Subscribe(roundObserver2);

        // Act
        sut.ProcessEvent(data);

        // Assert
        playerObserver1.Received(1).OnNext(Arg.Any<PlayerEvent>());
        playerStateObserver1.Received(1).OnNext(Arg.Any<PlayerStateEvent>());
        mapObserver1.Received(1).OnNext(Arg.Any<MapEvent>());
        roundObserver1.Received(1).OnNext(Arg.Any<RoundEvent>());

        playerObserver2.Received(1).OnNext(Arg.Any<PlayerEvent>());
        playerStateObserver2.Received(1).OnNext(Arg.Any<PlayerStateEvent>());
        mapObserver2.Received(1).OnNext(Arg.Any<MapEvent>());
        roundObserver2.Received(1).OnNext(Arg.Any<RoundEvent>());
    }

    [Theory, AutoNSubstituteData]
    internal void ProcessEvent_does_not_send_events_on_same_data(
        GameStateData data,
        IObserver<PlayerEvent> playerObserver,
        IObserver<PlayerStateEvent> playerStateObserver,
        IObserver<MapEvent> mapObserver,
        IObserver<RoundEvent> roundObserver,
        GameStateService sut)
    {
        // Arrange
        sut.ProcessEvent(data); // Set initial properties
        sut.Subscribe(playerObserver);
        sut.Subscribe(playerStateObserver);
        sut.Subscribe(mapObserver);
        sut.Subscribe(roundObserver);

        // Act
        sut.ProcessEvent(data); // Send same data again

        // Assert
        playerObserver.Received(0).OnNext(Arg.Any<PlayerEvent>());
        playerStateObserver.Received(0).OnNext(Arg.Any<PlayerStateEvent>());
        mapObserver.Received(0).OnNext(Arg.Any<MapEvent>());
        roundObserver.Received(0).OnNext(Arg.Any<RoundEvent>());
    }

    [Theory, AutoNSubstituteData]
    internal void ProcessEvent_does_not_send_events_to_unsubscribed_observers(
        GameStateData data1,
        GameStateData data2,
        IObserver<PlayerEvent> playerObserver,
        IObserver<PlayerStateEvent> playerStateObserver,
        IObserver<MapEvent> mapObserver,
        IObserver<RoundEvent> roundObserver,
        GameStateService sut)
    {
        // Arrange
        var playerSubscription = sut.Subscribe(playerObserver);
        var playerStateSubscription = sut.Subscribe(playerStateObserver);
        var mapSubscription = sut.Subscribe(mapObserver);
        var roundSubscription = sut.Subscribe(roundObserver);

        // Act
        sut.ProcessEvent(data1);
        playerSubscription.Dispose();
        playerStateSubscription.Dispose();
        mapSubscription.Dispose();
        roundSubscription.Dispose();
        sut.ProcessEvent(data2);

        // Assert
        playerObserver.Received(1).OnNext(Arg.Any<PlayerEvent>());
        playerStateObserver.Received(1).OnNext(Arg.Any<PlayerStateEvent>());
        mapObserver.Received(1).OnNext(Arg.Any<MapEvent>());
        roundObserver.Received(1).OnNext(Arg.Any<RoundEvent>());
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
    internal void GetPlayer_returns_null_on_unknown_SteamId(
        List<GameStateData> data,
        SteamId64 unknownSteamId,
        GameStateService sut)
    {
        // Arrange
        foreach (var @event in data)
        {
            sut.ProcessEvent(@event);
        }

        // Act
        var player = sut.GetPlayer(unknownSteamId);

        // Assert
        player.Should().BeNull();
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
    internal void GetMap_returns_null_on_unknown_SteamId(
        List<GameStateData> data,
        SteamId64 unknownSteamId,
        GameStateService sut)
    {
        // Arrange
        foreach (var @event in data)
        {
            sut.ProcessEvent(@event);
        }

        // Act
        var map = sut.GetMap(unknownSteamId);

        // Assert
        map.Should().BeNull();
    }

    [Theory, AutoData]
    internal void GetRound_returns_Round_by_SteamId(
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

    [Theory, AutoData]
    internal void GetRound_returns_null_on_unknown_SteamId(
        List<GameStateData> data,
        SteamId64 unknownSteamId,
        GameStateService sut)
    {
        // Arrange
        foreach (var @event in data)
        {
            sut.ProcessEvent(@event);
        }

        // Act
        var round = sut.GetRound(unknownSteamId);

        // Assert
        round.Should().BeNull();
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