using LupusBytes.CS2.GameStateIntegration.Contracts;

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
    internal void ProcessEvent_sends_StateUpdate_with_Player_to_observers(
        GameStateData data,
        IObserver<StateUpdate<Player>> observer,
        GameState sut)
    {
        // Arrange
        sut.Subscribe(observer);

        // Act
        sut.ProcessEvent(data);

        // Assert
        observer.Received(1).OnNext(Arg.Is<StateUpdate<Player>>(p =>
            p.SteamId == sut.SteamId &&
            p.State!.SteamId64 == data.Player!.SteamId64 &&
            p.State.Name == data.Player.Name &&
            p.State.Team == data.Player.Team &&
            p.State.Activity == data.Player.Activity));
    }

    [Theory, AutoNSubstituteData]
    internal void ProcessEvent_sends_StateUpdate_with_null_Player_to_observers(
        GameStateData data,
        IObserver<StateUpdate<Player>> observer,
        GameState sut)
    {
        // Arrange
        sut.ProcessEvent(data); // Set initial properties
        data = data with { Player = null };
        sut.Subscribe(observer);

        // Act
        sut.ProcessEvent(data);

        // Assert
        observer.Received(1).OnNext(Arg.Is<StateUpdate<Player>>(p =>
            p.SteamId == sut.SteamId &&
            p.State == null));
    }

    [Theory, AutoNSubstituteData]
    internal void ProcessEvent_sends_StateUpdate_with_PlayerState_to_observers(
        GameStateData data,
        IObserver<StateUpdate<PlayerState>> observer,
        GameState sut)
    {
        // Arrange
        sut.Subscribe(observer);

        // Act
        sut.ProcessEvent(data);

        // Assert
        observer.Received(1).OnNext(Arg.Is<StateUpdate<PlayerState>>(p =>
            p.SteamId == sut.SteamId &&
            p.State!.Health == data.Player!.State!.Health &&
            p.State.Armor == data.Player.State.Armor &&
            p.State.Helmet == data.Player.State.Helmet &&
            p.State.Flashed == data.Player.State.Flashed &&
            p.State.Smoked == data.Player.State.Smoked &&
            p.State.Burning == data.Player.State.Burning &&
            p.State.Money == data.Player.State.Money &&
            p.State.RoundKills == data.Player.State.RoundKills &&
            p.State.RoundKillHeadshots == data.Player.State.RoundKillHeadshots &&
            p.State.EquipmentValue == data.Player.State.EquipmentValue));
    }

    [Theory, AutoNSubstituteData]
    internal void ProcessEvent_sends_StateUpdate_with_null_PlayerState_to_observers(
        GameStateData data,
        IObserver<StateUpdate<PlayerState>> observer,
        GameState sut)
    {
        // Arrange
        sut.ProcessEvent(data); // Set initial properties
        data = data with { Player = data.Player! with { State = null } };
        sut.Subscribe(observer);

        // Act
        sut.ProcessEvent(data);

        // Assert
        observer.Received(1).OnNext(Arg.Is<StateUpdate<PlayerState>>(ps =>
            ps.SteamId == sut.SteamId &&
            ps.State == null));
    }

    [Theory, AutoNSubstituteData]
    internal void ProcessEvent_sends_StateUpdate_with_Map_to_observers(
        GameStateData data,
        IObserver<StateUpdate<Map>> observer,
        GameState sut)
    {
        // Arrange
        sut.Subscribe(observer);

        // Act
        sut.ProcessEvent(data);

        // Assert
        observer.Received(1).OnNext(Arg.Is<StateUpdate<Map>>(m =>
            m.SteamId == sut.SteamId &&
            m.State!.Mode == data.Map!.Mode &&
            m.State.Name == data.Map.Name &&
            m.State.Phase == data.Map.Phase &&
            m.State.Round == data.Map.Round &&
            m.State.T == data.Map.T &&
            m.State.CT == data.Map.CT));
    }

    [Theory, AutoNSubstituteData]
    internal void ProcessEvent_sends_StateUpdate_with_null_Map_to_observers(
        GameStateData data,
        IObserver<StateUpdate<Map>> observer,
        GameState sut)
    {
        // Arrange
        sut.ProcessEvent(data); // Set initial properties
        data = data with { Map = null };
        sut.Subscribe(observer);

        // Act
        sut.ProcessEvent(data);

        // Assert
        observer.Received(1).OnNext(Arg.Is<StateUpdate<Map>>(m =>
            m.SteamId == sut.SteamId &&
            m.State == null));
    }

    [Theory, AutoNSubstituteData]
    internal void ProcessEvent_sends_StateUpdate_with_Round_to_observers(
        GameStateData data,
        IObserver<StateUpdate<Round>> observer,
        GameState sut)
    {
        // Arrange
        sut.Subscribe(observer);

        // Act
        sut.ProcessEvent(data);

        // Assert
        observer.Received(1).OnNext(Arg.Is<StateUpdate<Round>>(r =>
            r.SteamId == sut.SteamId &&
            r.State!.Phase == data.Round!.Phase &&
            r.State.WinTeam == data.Round.WinTeam &&
            r.State.Bomb == data.Round.Bomb));
    }

    [Theory, AutoNSubstituteData]
    internal void ProcessEvent_sends_StateUpdate_with_null_Round_to_observers(
        GameStateData data,
        IObserver<StateUpdate<Round>> observer,
        GameState sut)
    {
        // Arrange
        sut.ProcessEvent(data); // Set initial properties
        data = data with { Round = null };
        sut.Subscribe(observer);

        // Act
        sut.ProcessEvent(data);

        // Assert
        observer.Received(1).OnNext(Arg.Is<StateUpdate<Round>>(r =>
            r.SteamId == sut.SteamId &&
            r.State == null));
    }

    [Theory, AutoNSubstituteData]
    internal void ProcessEvent_does_not_send_updates_on_same_data(
        GameStateData data,
        IObserver<StateUpdate<Player>> playerObserver,
        IObserver<StateUpdate<PlayerState>> playerStateObserver,
        IObserver<StateUpdate<Map>> mapObserver,
        IObserver<StateUpdate<Round>> roundObserver,
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
        playerObserver.Received(0).OnNext(Arg.Any<StateUpdate<Player>>());
        playerStateObserver.Received(0).OnNext(Arg.Any<StateUpdate<PlayerState>>());
        mapObserver.Received(0).OnNext(Arg.Any<StateUpdate<Map>>());
        roundObserver.Received(0).OnNext(Arg.Any<StateUpdate<Round>>());
    }

    [Theory, AutoNSubstituteData]
    internal void ProcessEvent_does_not_send_updates_to_unsubscribed_observers(
        GameStateData data1,
        GameStateData data2,
        IObserver<StateUpdate<Player>> playerObserver,
        IObserver<StateUpdate<PlayerState>> playerStateObserver,
        IObserver<StateUpdate<Map>> mapObserver,
        IObserver<StateUpdate<Round>> roundObserver,
        GameState sut)
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
        playerObserver.Received(1).OnNext(Arg.Any<StateUpdate<Player>>());
        playerStateObserver.Received(1).OnNext(Arg.Any<StateUpdate<PlayerState>>());
        mapObserver.Received(1).OnNext(Arg.Any<StateUpdate<Map>>());
        roundObserver.Received(1).OnNext(Arg.Any<StateUpdate<Round>>());
    }

    [Theory, AutoNSubstituteData]
    internal void ProcessEvent_sends_updates_to_multiple_observers(
        GameStateData data,
        IObserver<StateUpdate<Player>> playerObserver1,
        IObserver<StateUpdate<PlayerState>> playerStateObserver1,
        IObserver<StateUpdate<Map>> mapObserver1,
        IObserver<StateUpdate<Round>> roundObserver1,
        IObserver<StateUpdate<Player>> playerObserver2,
        IObserver<StateUpdate<PlayerState>> playerStateObserver2,
        IObserver<StateUpdate<Map>> mapObserver2,
        IObserver<StateUpdate<Round>> roundObserver2,
        GameState sut)
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
        playerObserver1.Received(1).OnNext(Arg.Any<StateUpdate<Player>>());
        playerStateObserver1.Received(1).OnNext(Arg.Any<StateUpdate<PlayerState>>());
        mapObserver1.Received(1).OnNext(Arg.Any<StateUpdate<Map>>());
        roundObserver1.Received(1).OnNext(Arg.Any<StateUpdate<Round>>());

        playerObserver2.Received(1).OnNext(Arg.Any<StateUpdate<Player>>());
        playerStateObserver2.Received(1).OnNext(Arg.Any<StateUpdate<PlayerState>>());
        mapObserver2.Received(1).OnNext(Arg.Any<StateUpdate<Map>>());
        roundObserver2.Received(1).OnNext(Arg.Any<StateUpdate<Round>>());
    }
}