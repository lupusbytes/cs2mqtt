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

        var observers = new Observers(sut);

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
        observers.AssertAllReceivedDataForSteamId(2, provider1.SteamId64);
        observers.AssertAllReceivedDataForSteamId(1, provider2.SteamId64);
    }

    [Theory, AutoNSubstituteData]
    internal void ProcessEvent_sends_events_to_multiple_observers(
        [Frozen] GameStateOptions options,
        GameStateData data,
        GameStateService sut)
    {
        // Arrange
        options.IgnoreSpectatedPlayers = false;
        var observer1 = new Observers(sut);
        var observer2 = new Observers(sut);

        // Act
        sut.ProcessEvent(data);

        // Assert
        observer1.AssertAllReceivedData(1);
        observer2.AssertAllReceivedData(1);
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

        var observers = new Observers(sut);

        // Act
        sut.ProcessEvent(data); // Send same data again

        // Assert
        observers.AssertAllReceivedData(0);
    }

    [Theory, AutoNSubstituteData]
    internal void ProcessEvent_does_not_send_events_to_unsubscribed_observers(
        [Frozen] GameStateOptions options,
        GameStateData data1,
        GameStateData data2,
        GameStateService sut)
    {
        // Arrange
        options.IgnoreSpectatedPlayers = false;

        var observers = new Observers(sut);

        // Act
        sut.ProcessEvent(data1);
        observers.DisposeAll();
        sut.ProcessEvent(data2);

        // Assert
        observers.AssertAllReceivedData(1);
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

        var observers = new Observers(sut);

        // Act
        sut.ProcessEvent(data);

        // Assert
        // After sending data for the SteamID to the service, it should be able return data to us for the same SteamID.
        sut.GetPlayer(data.Provider!.SteamId64).Should().NotBeNull();

        // Wait 1 second to allow the background cleanup task to perform its work.
        await Task.Delay(TimeSpan.FromSeconds(1), TestContext.Current.CancellationToken);

        // After the wait, the background cleanup task should have removed the provider.
        sut.GetPlayer(data.Provider.SteamId64).Should().BeNull();

        // All the observers should have received null events for the corresponding SteamID.
        observers.AssertAllReceivedNullForSteamId(data.Provider.SteamId64);
    }

    private sealed class Observers
    {
        private readonly IObserver<StateUpdate<Player>> playerObserver;
        private readonly IDisposable playerSubscription;

        private readonly IObserver<StateUpdate<PlayerState>> playerStateObserver;
        private readonly IDisposable playerStateSubscription;

        private readonly IObserver<StateUpdate<PlayerMatchStats>> playerMatchStatsObserver;
        private readonly IDisposable playerMatchStatsSubscription;

        private readonly IObserver<StateUpdate<Round>> roundObserver;
        private readonly IDisposable roundSubscription;

        private readonly IObserver<StateUpdate<Map>> mapObserver;
        private readonly IDisposable mapSubscription;

        public Observers(GameStateService sut)
        {
            playerObserver = Substitute.For<IObserver<StateUpdate<Player>>>();
            playerSubscription = sut.Subscribe(playerObserver);

            playerStateObserver = Substitute.For<IObserver<StateUpdate<PlayerState>>>();
            playerStateSubscription = sut.Subscribe(playerStateObserver);

            playerMatchStatsObserver = Substitute.For<IObserver<StateUpdate<PlayerMatchStats>>>();
            playerMatchStatsSubscription = sut.Subscribe(playerMatchStatsObserver);

            roundObserver = Substitute.For<IObserver<StateUpdate<Round>>>();
            roundSubscription = sut.Subscribe(roundObserver);

            mapObserver = Substitute.For<IObserver<StateUpdate<Map>>>();
            mapSubscription = sut.Subscribe(mapObserver);
        }

        public void AssertAllReceivedData(int receivedCount)
        {
            playerObserver.Received(receivedCount).OnNext(Arg.Any<StateUpdate<Player>>());
            playerStateObserver.Received(receivedCount).OnNext(Arg.Any<StateUpdate<PlayerState>>());
            playerMatchStatsObserver.Received(receivedCount).OnNext(Arg.Any<StateUpdate<PlayerMatchStats>>());
            mapObserver.Received(receivedCount).OnNext(Arg.Any<StateUpdate<Map>>());
            roundObserver.Received(receivedCount).OnNext(Arg.Any<StateUpdate<Round>>());
        }

        public void AssertAllReceivedDataForSteamId(int receivedCount, SteamId64 steamId)
        {
            playerObserver.Received(receivedCount).OnNext(Arg.Is<StateUpdate<Player>>(r => r.SteamId == steamId));
            playerStateObserver.Received(receivedCount).OnNext(Arg.Is<StateUpdate<PlayerState>>(r => r.SteamId == steamId));
            playerMatchStatsObserver.Received(receivedCount).OnNext(Arg.Is<StateUpdate<PlayerMatchStats>>(r => r.SteamId == steamId));
            mapObserver.Received(receivedCount).OnNext(Arg.Is<StateUpdate<Map>>(r => r.SteamId == steamId));
            roundObserver.Received(receivedCount).OnNext(Arg.Is<StateUpdate<Round>>(r => r.SteamId == steamId));
        }

        public void AssertAllReceivedNullForSteamId(SteamId64 steamId)
        {
            playerObserver.Received(1).OnNext(Arg.Is<StateUpdate<Player>>(p => p.SteamId == steamId && p.State == null));
            playerStateObserver.Received(1).OnNext(Arg.Is<StateUpdate<PlayerState>>(ps => ps.SteamId == steamId && ps.State == null));
            playerMatchStatsObserver.Received(1).OnNext(Arg.Is<StateUpdate<PlayerMatchStats>>(pms => pms.SteamId == steamId && pms.State == null));
            mapObserver.Received(1).OnNext(Arg.Is<StateUpdate<Map>>(m => m.SteamId == steamId && m.State == null));
            roundObserver.Received(1).OnNext(Arg.Is<StateUpdate<Round>>(r => r.SteamId == steamId && r.State == null));
        }

        public void DisposeAll()
        {
            playerSubscription.Dispose();
            playerStateSubscription.Dispose();
            playerMatchStatsSubscription.Dispose();
            mapSubscription.Dispose();
            roundSubscription.Dispose();
        }
    }
}