namespace LupusBytes.CS2.GameStateIntegration.Api.Extensions;

public static class ServiceProviderExtensions
{
    public static Task GetStopApplicationTask(this IServiceProvider sp)
    {
        sp.GetRequiredService<IHostApplicationLifetime>().StopApplication();
        return Task.CompletedTask;
    }
}