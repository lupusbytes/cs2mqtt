using LupusBytes.CS2.GameStateIntegration.Contracts;
using LupusBytes.CS2.GameStateIntegration.Events;

namespace LupusBytes.CS2.GameStateIntegration.Tests;

public class GameStateTest
{
    [Theory, AutoData]
    internal void ProcessEvent_sets_properties(GameStateData data, GameState sut)
    {
        // Act
        sut.ProcessEvent(data);

        // Assert
        sut.Player.Should().BeEquivalentTo(data.Player);
        sut.Map.Should().BeEquivalentTo(data.Map);
        sut.Round.Should().BeEquivalentTo(data.Round);
    }

    [Theory, AutoNSubstituteData]
    internal void ProcessEvent_sends_PlayerWithStateEvent_to_observers(
        GameStateData data,
        IObserver<PlayerWithStateEvent> observer,
        GameState sut)
    {
        // Arrange
        sut.Subscribe(observer);

        // Act
        sut.ProcessEvent(data);

        // Assert
        observer.Received(1).OnNext(Arg.Is<PlayerWithStateEvent>(p =>
            p.SteamId == sut.SteamId &&
            p.Player == data.Player));
    }

    [Theory, AutoNSubstituteData]
    internal void ProcessEvent_sends_PlayerWithStateEvent_with_null_Player_to_observers(
        GameStateData data,
        IObserver<PlayerWithStateEvent> observer,
        GameState sut)
    {
        // Arrange
        sut.ProcessEvent(data); // Set initial properties
        data = data with { Player = null };
        sut.Subscribe(observer);

        // Act
        sut.ProcessEvent(data);

        // Assert
        observer.Received(1).OnNext(Arg.Is<PlayerWithStateEvent>(p =>
            p.SteamId == sut.SteamId &&
            p.Player == null));
    }

    [Theory, AutoNSubstituteData]
    internal void ProcessEvent_sends_MapEvent_to_observers(
        GameStateData data,
        IObserver<MapEvent> observer,
        GameState sut)
    {
        // Arrange
        sut.Subscribe(observer);

        // Act
        sut.ProcessEvent(data);

        // Assert
        observer.Received(1).OnNext(Arg.Is<MapEvent>(m =>
            m.SteamId == sut.SteamId &&
            m.Map!.Mode == data.Map!.Mode &&
            m.Map.Name == data.Map.Name &&
            m.Map.Phase == data.Map.Phase &&
            m.Map.Round == data.Map.Round &&
            m.Map.T == data.Map.T &&
            m.Map.CT == data.Map.CT));
    }

    [Theory, AutoNSubstituteData]
    internal void ProcessEvent_sends_MapEvent_with_null_Map_to_observers(
        GameStateData data,
        IObserver<MapEvent> observer,
        GameState sut)
    {
        // Arrange
        sut.ProcessEvent(data); // Set initial properties
        data = data with { Map = null };
        sut.Subscribe(observer);

        // Act
        sut.ProcessEvent(data);

        // Assert
        observer.Received(1).OnNext(Arg.Is<MapEvent>(m =>
            m.SteamId == sut.SteamId &&
            m.Map == null));
    }

    [Theory, AutoNSubstituteData]
    internal void ProcessEvent_sends_RoundEvent_to_observers(
        GameStateData data,
        IObserver<RoundEvent> observer,
        GameState sut)
    {
        // Arrange
        sut.Subscribe(observer);

        // Act
        sut.ProcessEvent(data);

        // Assert
        observer.Received(1).OnNext(Arg.Is<RoundEvent>(r =>
            r.SteamId == sut.SteamId &&
            r.Round!.Phase == data.Round!.Phase &&
            r.Round.WinTeam == data.Round.WinTeam &&
            r.Round.Bomb == data.Round.Bomb));
    }

    [Theory, AutoNSubstituteData]
    internal void ProcessEvent_sends_RoundEvent_with_null_Round_to_observers(
        GameStateData data,
        IObserver<RoundEvent> observer,
        GameState sut)
    {
        // Arrange
        sut.ProcessEvent(data); // Set initial properties
        data = data with { Round = null };
        sut.Subscribe(observer);

        // Act
        sut.ProcessEvent(data);

        // Assert
        observer.Received(1).OnNext(Arg.Is<RoundEvent>(r =>
            r.SteamId == sut.SteamId &&
            r.Round == null));
    }

    [Theory, AutoNSubstituteData]
    internal void ProcessEvent_does_not_send_events_on_same_data(
        GameStateData data,
        IObserver<PlayerEvent> playerObserver,
        IObserver<PlayerStateEvent> playerStateObserver,
        IObserver<MapEvent> mapObserver,
        IObserver<RoundEvent> roundObserver,
        GameState sut)
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
        IObserver<PlayerWithStateEvent> playerObserver,
        IObserver<MapEvent> mapObserver,
        IObserver<RoundEvent> roundObserver,
        GameState sut)
    {
        // Arrange
        var playerSubscription = sut.Subscribe(playerObserver);
        var mapSubscription = sut.Subscribe(mapObserver);
        var roundSubscription = sut.Subscribe(roundObserver);

        // Act
        sut.ProcessEvent(data1);
        playerSubscription.Dispose();
        mapSubscription.Dispose();
        roundSubscription.Dispose();
        sut.ProcessEvent(data2);

        // Assert
        playerObserver.Received(1).OnNext(Arg.Any<PlayerWithStateEvent>());
        mapObserver.Received(1).OnNext(Arg.Any<MapEvent>());
        roundObserver.Received(1).OnNext(Arg.Any<RoundEvent>());
    }

    [Theory, AutoNSubstituteData]
    internal void ProcessEvent_sends_events_to_multiple_observers(
        GameStateData data,
        IObserver<PlayerWithStateEvent> playerObserver1,
        IObserver<MapEvent> mapObserver1,
        IObserver<RoundEvent> roundObserver1,
        IObserver<PlayerWithStateEvent> playerObserver2,
        IObserver<MapEvent> mapObserver2,
        IObserver<RoundEvent> roundObserver2,
        GameState sut)
    {
        // Arrange
        sut.Subscribe(playerObserver1);
        sut.Subscribe(mapObserver1);
        sut.Subscribe(roundObserver1);

        sut.Subscribe(playerObserver2);
        sut.Subscribe(mapObserver2);
        sut.Subscribe(roundObserver2);

        // Act
        sut.ProcessEvent(data);

        // Assert
        playerObserver1.Received(1).OnNext(Arg.Any<PlayerWithStateEvent>());
        mapObserver1.Received(1).OnNext(Arg.Any<MapEvent>());
        roundObserver1.Received(1).OnNext(Arg.Any<RoundEvent>());

        playerObserver2.Received(1).OnNext(Arg.Any<PlayerWithStateEvent>());
        mapObserver2.Received(1).OnNext(Arg.Any<MapEvent>());
        roundObserver2.Received(1).OnNext(Arg.Any<RoundEvent>());
    }
}