using LupusBytes.CS2.GameStateIntegration.Api.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace LupusBytes.CS2.GameStateIntegration.Api.Tests.Extensions;

public class ServiceProviderExtensionsTest
{
    [Fact]
    public async Task GetStopApplicationTask_returns_IHostApplicationLifetime_StopApplication_Task()
    {
        // Arrange
        var lifetime = Substitute.For<IHostApplicationLifetime>();
        var services = new ServiceCollection();
        services.AddSingleton(lifetime);
        var serviceProvider = services.BuildServiceProvider();

        // Act
        await serviceProvider.GetStopApplicationTask();

        // Assert
        lifetime.Received(1).StopApplication();
    }
}