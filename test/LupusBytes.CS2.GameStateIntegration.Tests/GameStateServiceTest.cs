using LupusBytes.CS2.GameStateIntegration.Contracts;

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
        [Frozen] GameStateOptions options,
        Provider provider1,
        Provider provider2,
        GameStateData data1,
        GameStateData data2,
        GameStateData data3,
        GameStateService sut)
    {
        // Arrange
        options.IgnoreSpectatedPlayers = false;

        var subscriber = new Subscriber(sut);

        var data = new List<GameStateData>
        {
            data1 with { Provider = provider1 },
            data2 with { Provider = provider2 },
            data3 with { Provider = provider1 },
        };

        // Act
        foreach (var @event in data)
        {
            sut.ProcessEvent(@event);
        }

        // Assert
        subscriber.AssertAllReceivedDataForSteamId(2, provider1.SteamId64);
        subscriber.AssertAllReceivedDataForSteamId(1, provider2.SteamId64);
    }

    [Theory, AutoNSubstituteData]
    internal void ProcessEvent_sends_events_to_multiple_subscribers(
        [Frozen] GameStateOptions options,
        GameStateData data,
        GameStateService sut)
    {
        // Arrange
        options.IgnoreSpectatedPlayers = false;
        var subscriber1 = new Subscriber(sut);
        var subscriber2 = new Subscriber(sut);

        // Act
        sut.ProcessEvent(data);

        // Assert
        subscriber1.AssertAllReceivedData(1);
        subscriber2.AssertAllReceivedData(1);
    }

    [Theory, AutoNSubstituteData]
    internal void ProcessEvent_does_not_send_events_on_same_data(
        [Frozen] GameStateOptions options,
        GameStateData data,
        GameStateService sut)
    {
        // Arrange
        options.IgnoreSpectatedPlayers = false;

        sut.ProcessEvent(data); // Set initial properties

        var subscriber = new Subscriber(sut);

        // Act
        sut.ProcessEvent(data); // Send same data again

        // Assert
        subscriber.AssertAllReceivedData(0);
    }

    [Theory, AutoNSubstituteData]
    internal void ProcessEvent_does_not_send_events_to_unsubscribed_handlers(
        [Frozen] GameStateOptions options,
        GameStateData data1,
        GameStateData data2,
        GameStateService sut)
    {
        // Arrange
        options.IgnoreSpectatedPlayers = false;

        var subscriber = new Subscriber(sut);

        // Act
        sut.ProcessEvent(data1);
        subscriber.UnsubscribeAll();
        sut.ProcessEvent(data2);

        // Assert
        subscriber.AssertAllReceivedData(1);
    }

    [Theory, AutoData]
    internal void GetPlayer_returns_Player_by_SteamId(
        [Frozen] GameStateOptions options,
        GameStateData data1,
        GameStateData data2,
        GameStateData data3,
        GameStateService sut)
    {
        // Arrange
        options.IgnoreSpectatedPlayers = false;
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

    [Theory, AutoData]
    public async Task Removes_disconnected_providers_in_background(GameStateData data)
    {
        // Arrange
        var options = new GameStateOptions
        {
            TimeoutInSeconds = 0.2,
            TimeoutCleanupIntervalInSeconds = 0.5,
            IgnoreSpectatedPlayers = false,
        };

        var sut = new GameStateService(options);

        var subscriber = new Subscriber(sut);

        // Act
        sut.ProcessEvent(data);

        // Assert
        // After sending data for the SteamID to the service, it should be able return data to us for the same SteamID.
        sut.GetPlayer(data.Provider!.SteamId64).Should().NotBeNull();

        // Wait 1 second to allow the background cleanup task to perform its work.
        await Task.Delay(TimeSpan.FromSeconds(1), TestContext.Current.CancellationToken);

        // After the wait, the background cleanup task should have removed the provider.
        sut.GetPlayer(data.Provider.SteamId64).Should().BeNull();

        // All the subscribers should have received null events for the corresponding SteamID.
        subscriber.AssertAllReceivedNullForSteamId(data.Provider.SteamId64);
    }

    /// <summary>
    /// Attaches a substituted handler to every state update event of a <see cref="GameStateService"/>.
    /// </summary>
    private sealed class Subscriber
    {
        private readonly GameStateService gameStateService;
        private readonly EventHandler<StateUpdateEventArgs<Player>> playerHandler = Substitute.For<EventHandler<StateUpdateEventArgs<Player>>>();
        private readonly EventHandler<StateUpdateEventArgs<PlayerState>> playerStateHandler = Substitute.For<EventHandler<StateUpdateEventArgs<PlayerState>>>();
        private readonly EventHandler<StateUpdateEventArgs<PlayerMatchStats>> playerMatchStatsHandler = Substitute.For<EventHandler<StateUpdateEventArgs<PlayerMatchStats>>>();
        private readonly EventHandler<StateUpdateEventArgs<Round>> roundHandler = Substitute.For<EventHandler<StateUpdateEventArgs<Round>>>();
        private readonly EventHandler<StateUpdateEventArgs<Map>> mapHandler = Substitute.For<EventHandler<StateUpdateEventArgs<Map>>>();

        public Subscriber(GameStateService gameStateService)
        {
            this.gameStateService = gameStateService;
            gameStateService.PlayerUpdated += playerHandler;
            gameStateService.PlayerStateUpdated += playerStateHandler;
            gameStateService.PlayerMatchStatsUpdated += playerMatchStatsHandler;
            gameStateService.RoundUpdated += roundHandler;
            gameStateService.MapUpdated += mapHandler;
        }

        public void AssertAllReceivedData(int receivedCount)
        {
            playerHandler.Received(receivedCount).Invoke(Arg.Any<object>(), Arg.Any<StateUpdateEventArgs<Player>>());
            playerStateHandler.Received(receivedCount).Invoke(Arg.Any<object>(), Arg.Any<StateUpdateEventArgs<PlayerState>>());
            playerMatchStatsHandler.Received(receivedCount).Invoke(Arg.Any<object>(), Arg.Any<StateUpdateEventArgs<PlayerMatchStats>>());
            mapHandler.Received(receivedCount).Invoke(Arg.Any<object>(), Arg.Any<StateUpdateEventArgs<Map>>());
            roundHandler.Received(receivedCount).Invoke(Arg.Any<object>(), Arg.Any<StateUpdateEventArgs<Round>>());
        }

        public void AssertAllReceivedDataForSteamId(int receivedCount, SteamId64 steamId)
        {
            playerHandler.Received(receivedCount).Invoke(Arg.Any<object>(), Arg.Is<StateUpdateEventArgs<Player>>(r => r.SteamId == steamId));
            playerStateHandler.Received(receivedCount).Invoke(Arg.Any<object>(), Arg.Is<StateUpdateEventArgs<PlayerState>>(r => r.SteamId == steamId));
            playerMatchStatsHandler.Received(receivedCount).Invoke(Arg.Any<object>(), Arg.Is<StateUpdateEventArgs<PlayerMatchStats>>(r => r.SteamId == steamId));
            mapHandler.Received(receivedCount).Invoke(Arg.Any<object>(), Arg.Is<StateUpdateEventArgs<Map>>(r => r.SteamId == steamId));
            roundHandler.Received(receivedCount).Invoke(Arg.Any<object>(), Arg.Is<StateUpdateEventArgs<Round>>(r => r.SteamId == steamId));
        }

        public void AssertAllReceivedNullForSteamId(SteamId64 steamId)
        {
            playerHandler.Received(1).Invoke(Arg.Any<object>(), Arg.Is<StateUpdateEventArgs<Player>>(p => p.SteamId == steamId && p.State == null));
            playerStateHandler.Received(1).Invoke(Arg.Any<object>(), Arg.Is<StateUpdateEventArgs<PlayerState>>(ps => ps.SteamId == steamId && ps.State == null));
            playerMatchStatsHandler.Received(1).Invoke(Arg.Any<object>(), Arg.Is<StateUpdateEventArgs<PlayerMatchStats>>(pms => pms.SteamId == steamId && pms.State == null));
            mapHandler.Received(1).Invoke(Arg.Any<object>(), Arg.Is<StateUpdateEventArgs<Map>>(m => m.SteamId == steamId && m.State == null));
            roundHandler.Received(1).Invoke(Arg.Any<object>(), Arg.Is<StateUpdateEventArgs<Round>>(r => r.SteamId == steamId && r.State == null));
        }

        public void UnsubscribeAll()
        {
            gameStateService.PlayerUpdated -= playerHandler;
            gameStateService.PlayerStateUpdated -= playerStateHandler;
            gameStateService.PlayerMatchStatsUpdated -= playerMatchStatsHandler;
            gameStateService.MapUpdated -= mapHandler;
            gameStateService.RoundUpdated -= roundHandler;
        }
    }
}
