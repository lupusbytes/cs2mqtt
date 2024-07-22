using LupusBytes.CS2.GameStateIntegration.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace LupusBytes.CS2.GameStateIntegration.Tests.Extensions;

public class ServiceCollectionExtensionsTest
{
    [Theory, AutoNSubstituteData]
    public void AddGameStateService_registers_GameStateService(IConfiguration configuration)
    {
        // Arrange
        var sut = new ServiceCollection();

        // Act
        sut.AddGameStateService(configuration);

        // Assert
        sut.BuildServiceProvider()
            .GetRequiredService<IGameStateService>()
            .Should()
            .BeOfType<GameStateService>();
    }
}