using LupusBytes.CS2.GameStateIntegration.Contracts;

namespace LupusBytes.CS2.GameStateIntegration.Api.Events;

public class ProviderEventArgs(Provider provider) : EventArgs
{
    public Provider Provider { get; } = provider;
}