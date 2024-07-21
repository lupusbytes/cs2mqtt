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
    internal void ProcessEvent_sends_PlayerEvent_to_observers(
        GameStateData data,
        IObserver<PlayerEvent> observer,
        GameState sut)
    {
        // Arrange
        sut.Subscribe(observer);

        // Act
        sut.ProcessEvent(data);

        // Assert
        observer.Received(1).OnNext(Arg.Is<PlayerEvent>(p =>
            p.SteamId == sut.SteamId &&
            p.Player!.SteamId64 == data.Player!.SteamId64 &&
            p.Player.Name == data.Player.Name &&
            p.Player.Team == data.Player.Team &&
            p.Player.Activity == data.Player.Activity));
    }

    [Theory, AutoNSubstituteData]
    internal void ProcessEvent_sends_PlayerStateEvent_to_observers(
        GameStateData data,
        IObserver<PlayerStateEvent> observer,
        GameState sut)
    {
        // Arrange
        sut.Subscribe(observer);

        // Act
        sut.ProcessEvent(data);

        // Assert
        observer.Received(1).OnNext(Arg.Is<PlayerStateEvent>(p =>
            p.SteamId == sut.SteamId &&
            p.PlayerState!.Health == data.Player!.State!.Health &&
            p.PlayerState.Armor == data.Player.State.Armor &&
            p.PlayerState.Helmet == data.Player.State.Helmet &&
            p.PlayerState.Flashed == data.Player.State.Flashed &&
            p.PlayerState.Smoked == data.Player.State.Smoked &&
            p.PlayerState.Burning == data.Player.State.Burning &&
            p.PlayerState.Money == data.Player.State.Money &&
            p.PlayerState.RoundKills == data.Player.State.RoundKills &&
            p.PlayerState.RoundKillHeadshots == data.Player.State.RoundKillHeadshots &&
            p.PlayerState.EquipmentValue == data.Player.State.EquipmentValue));
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
}