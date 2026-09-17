using LupusBytes.CS2.GameStateIntegration.Contracts;

namespace LupusBytes.CS2.GameStateIntegration.Tests;

public class GameStateTest
{
    [Theory, AutoNSubstituteData]
    internal void ProcessEvent_sets_properties(
        GameStateData data,
        IGameStateUpdateListener listener)
    {
        // Arrange
        var sut = new GameState(data.Provider!.SteamId64, ignoreSpectatedPlayers: false, listener);

        // Act
        sut.ProcessEvent(data);

        // Assert
        sut.Player.Should().BeEquivalentTo(data.Player);
        sut.Map.Should().BeEquivalentTo(data.Map);
        sut.Round.Should().BeEquivalentTo(data.Round);
    }

    [Theory, AutoNSubstituteData]
    internal void ProcessEvent_notifies_Player_while_IgnoreSpectatedPlayers_false(
        GameStateData data,
        IGameStateUpdateListener listener)
    {
        // Arrange
        var sut = new GameState(data.Provider!.SteamId64, ignoreSpectatedPlayers: false, listener);

        // Act
        sut.ProcessEvent(data);

        // Assert
        listener.Received(1).OnPlayerUpdated(Arg.Is<StateUpdateEventArgs<Player>>(p =>
            p.SteamId == sut.SteamId &&
            p.State!.SteamId64 == data.Player!.SteamId64 &&
            p.State.Name == data.Player.Name &&
            p.State.Team == data.Player.Team &&
            p.State.Activity == data.Player.Activity));
    }

    [Theory, AutoNSubstituteData]
    internal void ProcessEvent_notifies_Player_while_IgnoreSpectatedPlayers_true_and_SteamId_matches(
        GameStateData data,
        IGameStateUpdateListener listener)
    {
        // Arrange
        var sut = new GameState(data.Provider!.SteamId64, ignoreSpectatedPlayers: true, listener);
        data = data with { Player = data.Player! with { SteamId64 = data.Provider.SteamId64 } };

        // Act
        sut.ProcessEvent(data);

        // Assert
        listener.Received(1).OnPlayerUpdated(Arg.Is<StateUpdateEventArgs<Player>>(p =>
            p.SteamId == sut.SteamId &&
            p.State!.SteamId64 == data.Player.SteamId64 &&
            p.State.Name == data.Player.Name &&
            p.State.Team == data.Player.Team &&
            p.State.Activity == data.Player.Activity));
    }

    [Theory, AutoNSubstituteData]
    internal void ProcessEvent_notifies_null_Player_while_IgnoreSpectatedPlayers_false(
        GameStateData data,
        IGameStateUpdateListener listener)
    {
        // Arrange
        var sut = new GameState(data.Provider!.SteamId64, ignoreSpectatedPlayers: false, listener);
        sut.ProcessEvent(data); // Set initial properties
        data = data with { Player = null };
        listener.ClearReceivedCalls();

        // Act
        sut.ProcessEvent(data);

        // Assert
        listener.Received(1).OnPlayerUpdated(Arg.Is<StateUpdateEventArgs<Player>>(p =>
            p.SteamId == sut.SteamId &&
            p.State == null));
    }

    [Theory, AutoNSubstituteData]
    internal void ProcessEvent_does_not_notify_null_Player_while_IgnoreSpectatedPlayers_true_and_Player_is_null(
        GameStateData data,
        IGameStateUpdateListener listener)
    {
        // Arrange
        var sut = new GameState(data.Provider!.SteamId64, ignoreSpectatedPlayers: true, listener);
        data = data with { Player = data.Player! with { SteamId64 = data.Provider.SteamId64 } };
        sut.ProcessEvent(data); // Set initial properties with matching SteamId
        data = data with { Player = null };
        listener.ClearReceivedCalls();

        // Act
        sut.ProcessEvent(data);

        // Assert
        listener.Received(0).OnPlayerUpdated(Arg.Is<StateUpdateEventArgs<Player>>(p =>
            p.SteamId == sut.SteamId &&
            p.State == null));
    }

    [Theory, AutoNSubstituteData]
    internal void ProcessEvent_does_not_notify_Player_when_IgnoreSpectatedPlayers_true_and_SteamId_doesnt_match(
        GameStateData data,
        SteamId64 differentSteamId,
        IGameStateUpdateListener listener)
    {
        // Arrange
        var sut = new GameState(data.Provider!.SteamId64, ignoreSpectatedPlayers: true, listener);
        data = data with { Player = data.Player! with { SteamId64 = differentSteamId.ToString() } };

        // Act
        sut.ProcessEvent(data);

        // Assert
        listener.Received(0).OnPlayerUpdated(Arg.Any<StateUpdateEventArgs<Player>>());
    }

    [Theory, AutoNSubstituteData]
    internal void ProcessEvent_notifies_PlayerState_while_IgnoreSpectatedPlayers_false(
        GameStateData data,
        IGameStateUpdateListener listener)
    {
        // Arrange
        var sut = new GameState(data.Provider!.SteamId64, ignoreSpectatedPlayers: false, listener);

        // Act
        sut.ProcessEvent(data);

        // Assert
        listener.Received(1).OnPlayerStateUpdated(Arg.Is<StateUpdateEventArgs<PlayerState>>(p =>
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
    internal void ProcessEvent_notifies_null_PlayerState_while_IgnoreSpectatedPlayers_false(
        GameStateData data,
        IGameStateUpdateListener listener)
    {
        // Arrange
        var sut = new GameState(data.Provider!.SteamId64, ignoreSpectatedPlayers: false, listener);
        sut.ProcessEvent(data); // Set initial properties
        data = data with { Player = data.Player! with { State = null } };
        listener.ClearReceivedCalls();

        // Act
        sut.ProcessEvent(data);

        // Assert
        listener.Received(1).OnPlayerStateUpdated(Arg.Is<StateUpdateEventArgs<PlayerState>>(ps =>
            ps.SteamId == sut.SteamId &&
            ps.State == null));
    }

    [Theory, AutoNSubstituteData]
    internal void ProcessEvent_notifies_PlayerState_while_IgnoreSpectatedPlayers_true(
        GameStateData data,
        IGameStateUpdateListener listener)
    {
        // Arrange
        var sut = new GameState(data.Provider!.SteamId64, ignoreSpectatedPlayers: true, listener);
        data = data with { Player = data.Player! with { SteamId64 = data.Provider.SteamId64 } };

        // Act
        sut.ProcessEvent(data);

        // Assert
        listener.Received(1).OnPlayerStateUpdated(Arg.Is<StateUpdateEventArgs<PlayerState>>(ps =>
            ps.SteamId == sut.SteamId &&
            ps.State!.Health == data.Player.State!.Health &&
            ps.State.Armor == data.Player.State.Armor &&
            ps.State.Helmet == data.Player.State.Helmet &&
            ps.State.Flashed == data.Player.State.Flashed &&
            ps.State.Smoked == data.Player.State.Smoked &&
            ps.State.Burning == data.Player.State.Burning &&
            ps.State.Money == data.Player.State.Money &&
            ps.State.RoundKills == data.Player.State.RoundKills &&
            ps.State.RoundKillHeadshots == data.Player.State.RoundKillHeadshots &&
            ps.State.EquipmentValue == data.Player.State.EquipmentValue));
    }

    [Theory, AutoNSubstituteData]
    internal void ProcessEvent_notifies_null_PlayerState_while_IgnoreSpectatedPlayers_true(
        GameStateData data,
        IGameStateUpdateListener listener)
    {
        // Arrange
        var sut = new GameState(data.Provider!.SteamId64, ignoreSpectatedPlayers: true, listener);
        data = data with { Player = data.Player! with { SteamId64 = data.Provider.SteamId64 } };
        sut.ProcessEvent(data); // Set initial properties with matching SteamId
        data = data with { Player = data.Player with { State = null } };
        listener.ClearReceivedCalls();

        // Act
        sut.ProcessEvent(data);

        // Assert
        listener.Received(1).OnPlayerStateUpdated(Arg.Is<StateUpdateEventArgs<PlayerState>>(ps =>
            ps.SteamId == sut.SteamId &&
            ps.State == null));
    }

    [Theory, AutoNSubstituteData]
    internal void ProcessEvent_does_not_notify_PlayerState_when_IgnoreSpectatedPlayers_true_and_SteamId_doesnt_match(
        GameStateData data,
        SteamId64 differentSteamId,
        IGameStateUpdateListener listener)
    {
        // Arrange
        var sut = new GameState(data.Provider!.SteamId64, ignoreSpectatedPlayers: true, listener);
        data = data with { Player = data.Player! with { SteamId64 = differentSteamId.ToString() } };

        // Act
        sut.ProcessEvent(data);

        // Assert
        listener.Received(0).OnPlayerStateUpdated(Arg.Any<StateUpdateEventArgs<PlayerState>>());
    }

    [Theory, AutoNSubstituteData]
    internal void ProcessEvent_notifies_PlayerMatchStats_while_IgnoreSpectatedPlayers_false(
        GameStateData data,
        IGameStateUpdateListener listener)
    {
        // Arrange
        var sut = new GameState(data.Provider!.SteamId64, ignoreSpectatedPlayers: false, listener);

        // Act
        sut.ProcessEvent(data);

        // Assert
        listener.Received(1).OnPlayerMatchStatsUpdated(Arg.Is<StateUpdateEventArgs<PlayerMatchStats>>(p =>
            p.SteamId == sut.SteamId &&
            p.State!.Kills == data.Player!.MatchStats!.Kills &&
            p.State.Assists == data.Player.MatchStats.Assists &&
            p.State.Deaths == data.Player.MatchStats.Deaths &&
            p.State.Mvps == data.Player.MatchStats.Mvps &&
            p.State.Score == data.Player.MatchStats.Score));
    }

    [Theory, AutoNSubstituteData]
    internal void ProcessEvent_notifies_null_PlayerMatchStats_while_IgnoreSpectatedPlayers_false(
        GameStateData data,
        IGameStateUpdateListener listener)
    {
        // Arrange
        var sut = new GameState(data.Provider!.SteamId64, ignoreSpectatedPlayers: false, listener);
        sut.ProcessEvent(data); // Set initial properties
        data = data with { Player = data.Player! with { MatchStats = null } };
        listener.ClearReceivedCalls();

        // Act
        sut.ProcessEvent(data);

        // Assert
        listener.Received(1).OnPlayerMatchStatsUpdated(Arg.Is<StateUpdateEventArgs<PlayerMatchStats>>(ps =>
            ps.SteamId == sut.SteamId &&
            ps.State == null));
    }

    [Theory, AutoNSubstituteData]
    internal void ProcessEvent_notifies_PlayerMatchStats_while_IgnoreSpectatedPlayers_true(
        GameStateData data,
        IGameStateUpdateListener listener)
    {
        // Arrange
        var sut = new GameState(data.Provider!.SteamId64, ignoreSpectatedPlayers: true, listener);
        data = data with { Player = data.Player! with { SteamId64 = data.Provider.SteamId64 } };

        // Act
        sut.ProcessEvent(data);

        // Assert
        listener.Received(1).OnPlayerMatchStatsUpdated(Arg.Is<StateUpdateEventArgs<PlayerMatchStats>>(ps =>
            ps.SteamId == sut.SteamId &&
            ps.State!.Kills == data.Player.MatchStats!.Kills &&
            ps.State.Assists == data.Player.MatchStats.Assists &&
            ps.State.Deaths == data.Player.MatchStats.Deaths &&
            ps.State.Mvps == data.Player.MatchStats.Mvps &&
            ps.State.Score == data.Player.MatchStats.Score));
    }

    [Theory, AutoNSubstituteData]
    internal void ProcessEvent_notifies_null_PlayerMatchStats_while_IgnoreSpectatedPlayers_true(
        GameStateData data,
        IGameStateUpdateListener listener)
    {
        // Arrange
        var sut = new GameState(data.Provider!.SteamId64, ignoreSpectatedPlayers: true, listener);
        data = data with { Player = data.Player! with { SteamId64 = data.Provider.SteamId64 } };
        sut.ProcessEvent(data); // Set initial properties with matching SteamId
        data = data with { Player = data.Player with { MatchStats = null } };
        listener.ClearReceivedCalls();

        // Act
        sut.ProcessEvent(data);

        // Assert
        listener.Received(1).OnPlayerMatchStatsUpdated(Arg.Is<StateUpdateEventArgs<PlayerMatchStats>>(ps =>
            ps.SteamId == sut.SteamId &&
            ps.State == null));
    }

    [Theory, AutoNSubstituteData]
    internal void ProcessEvent_does_not_notify_PlayerMatchStats_when_IgnoreSpectatedPlayers_true_and_SteamId_doesnt_match(
        GameStateData data,
        SteamId64 differentSteamId,
        IGameStateUpdateListener listener)
    {
        // Arrange
        var sut = new GameState(data.Provider!.SteamId64, ignoreSpectatedPlayers: true, listener);
        data = data with { Player = data.Player! with { SteamId64 = differentSteamId.ToString() } };

        // Act
        sut.ProcessEvent(data);

        // Assert
        listener.Received(0).OnPlayerMatchStatsUpdated(Arg.Any<StateUpdateEventArgs<PlayerMatchStats>>());
    }

    [Theory, AutoNSubstituteData]
    internal void ProcessEvent_notifies_Map(
        GameStateData data,
        [Frozen] IGameStateUpdateListener listener,
        GameState sut)
    {
        // Act
        sut.ProcessEvent(data);

        // Assert
        listener.Received(1).OnMapUpdated(Arg.Is<StateUpdateEventArgs<Map>>(m =>
            m.SteamId == sut.SteamId &&
            m.State!.Mode == data.Map!.Mode &&
            m.State.Name == data.Map.Name &&
            m.State.Phase == data.Map.Phase &&
            m.State.Round == data.Map.Round &&
            m.State.T == data.Map.T &&
            m.State.CT == data.Map.CT));
    }

    [Theory, AutoNSubstituteData]
    internal void ProcessEvent_notifies_null_Map(
        GameStateData data,
        [Frozen] IGameStateUpdateListener listener,
        GameState sut)
    {
        // Arrange
        sut.ProcessEvent(data); // Set initial properties
        data = data with { Map = null };
        listener.ClearReceivedCalls();

        // Act
        sut.ProcessEvent(data);

        // Assert
        listener.Received(1).OnMapUpdated(Arg.Is<StateUpdateEventArgs<Map>>(m =>
            m.SteamId == sut.SteamId &&
            m.State == null));
    }

    [Theory, AutoNSubstituteData]
    internal void ProcessEvent_notifies_Round(
        GameStateData data,
        [Frozen] IGameStateUpdateListener listener,
        GameState sut)
    {
        // Act
        sut.ProcessEvent(data);

        // Assert
        listener.Received(1).OnRoundUpdated(Arg.Is<StateUpdateEventArgs<Round>>(r =>
            r.SteamId == sut.SteamId &&
            r.State!.Phase == data.Round!.Phase &&
            r.State.WinTeam == data.Round.WinTeam &&
            r.State.Bomb == data.Round.Bomb));
    }

    [Theory, AutoNSubstituteData]
    internal void ProcessEvent_notifies_null_Round(
        GameStateData data,
        [Frozen] IGameStateUpdateListener listener,
        GameState sut)
    {
        // Arrange
        sut.ProcessEvent(data); // Set initial properties
        data = data with { Round = null };
        listener.ClearReceivedCalls();

        // Act
        sut.ProcessEvent(data);

        // Assert
        listener.Received(1).OnRoundUpdated(Arg.Is<StateUpdateEventArgs<Round>>(r =>
            r.SteamId == sut.SteamId &&
            r.State == null));
    }

    [Theory, AutoNSubstituteData]
    internal void ProcessEvent_does_not_notify_on_same_data(
        GameStateData data,
        [Frozen] IGameStateUpdateListener listener,
        GameState sut)
    {
        // Arrange
        sut.ProcessEvent(data); // Set initial properties
        listener.ClearReceivedCalls();

        // Act
        sut.ProcessEvent(data); // Send same data again

        // Assert
        listener.Received(0).OnPlayerUpdated(Arg.Any<StateUpdateEventArgs<Player>>());
        listener.Received(0).OnPlayerStateUpdated(Arg.Any<StateUpdateEventArgs<PlayerState>>());
        listener.Received(0).OnPlayerMatchStatsUpdated(Arg.Any<StateUpdateEventArgs<PlayerMatchStats>>());
        listener.Received(0).OnMapUpdated(Arg.Any<StateUpdateEventArgs<Map>>());
        listener.Received(0).OnRoundUpdated(Arg.Any<StateUpdateEventArgs<Round>>());
    }

    [Theory, AutoNSubstituteData]
    internal void ProcessEvent_always_notifies_Provider(
        GameStateData data,
        [Frozen] IGameStateUpdateListener listener,
        GameState sut)
    {
        // Arrange
        sut.ProcessEvent(data); // Set initial properties
        listener.ClearReceivedCalls();

        // Act
        sut.ProcessEvent(data); // Send the same data again

        // Assert
        // The provider update is what keeps the connection alive, so it is sent even when nothing changed.
        listener.Received(1).OnProviderUpdated(Arg.Is<StateUpdateEventArgs<Provider>>(p =>
            p.SteamId == sut.SteamId &&
            p.State == data.Provider));
    }
}
