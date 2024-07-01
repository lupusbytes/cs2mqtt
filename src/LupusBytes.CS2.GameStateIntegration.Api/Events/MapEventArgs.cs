using LupusBytes.CS2.GameStateIntegration.Contracts;

namespace LupusBytes.CS2.GameStateIntegration.Api.Events;

public class MapEventArgs(Map? map) : EventArgs
{
    public Map? Map { get; } = map;
}