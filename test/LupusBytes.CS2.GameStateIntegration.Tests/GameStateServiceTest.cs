using AutoFixture.Xunit2;
using FluentAssertions;
using LupusBytes.CS2.GameStateIntegration.Contracts;

namespace LupusBytes.CS2.GameStateIntegration.Tests;

public class GameStateServiceTest
{
    [Theory, AutoData]
    public async Task Removes_disconnected_providers_in_background(GameStateData data)
    {
        // Arrange
        var options = new GameStateOptions
        {
            TimeoutInSeconds = 0.2,
            TimeoutCleanupIntervalInSeconds = 0.5,
        };
        var sut = new GameStateService(options);

        // Act
        sut.ProcessEvent(data);

        // Assert
        // After sending data for the SteamID to the service, it should be able return data to us for the same SteamID.
        sut.GetPlayer(data.Provider!.SteamId64).Should().NotBeNull();

        // Wait 1 second to allow the background cleanup task to perform its work.
        await Task.Delay(TimeSpan.FromSeconds(1));

        // After the wait, the background cleanup task should have removed the provider.
        sut.GetPlayer(data.Provider.SteamId64).Should().BeNull();
    }
}