using BenchmarkDotNet.Attributes;
using LupusBytes.CS2.GameStateIntegration.Contracts;

namespace LupusBytes.CS2.GameStateIntegration.Benchmarks;

/// <summary>
/// Measures the full production path: an event enters <see cref="GameStateService"/>,
/// is routed to the per-SteamID <see cref="GameState"/>, which pushes state updates back
/// to the service, which in turn fans them out to the application's subscribers.
/// </summary>
[MemoryDiagnoser]
public class GameStateServiceBenchmarks
{
    private GameStateService service = null!;
    private GameStateData[] events = null!;
    private StateUpdateSink[] sinks = null!;
    private int index;

    /// <summary>
    /// Production has three subscribers on the service:
    /// GameStateMqttPublisher, AvailabilityMqttPublisher and HomeAssistantDevicePublisher.
    /// </summary>
    [Params(1, 3)]
    public int SubscriberCount { get; set; }

    /// <summary>
    /// The number of distinct CS2 clients sending events to the service.
    /// </summary>
    [Params(1, 8)]
    public int ProviderCount { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        service = new GameStateService(new GameStateOptions
        {
            IgnoreSpectatedPlayers = false,

            // Keep the background cleanup task dormant for the duration of the run.
            TimeoutInSeconds = 60 * 60,
            TimeoutCleanupIntervalInSeconds = 60 * 60,
        });

        sinks = new StateUpdateSink[SubscriberCount];
        for (var i = 0; i < SubscriberCount; i++)
        {
            sinks[i] = new StateUpdateSink();
            sinks[i].SubscribeTo(service);
        }

        // Two events per provider, alternated, so that every block changes on every event.
        events = new GameStateData[ProviderCount * 2];
        for (var i = 0; i < ProviderCount; i++)
        {
            var steamId = GameStateDataFactory.SteamId(i);
            events[i * 2] = GameStateDataFactory.Create(steamId, seed: 1);
            events[(i * 2) + 1] = GameStateDataFactory.Create(steamId, seed: 2);
        }

        foreach (var @event in events)
        {
            service.ProcessEvent(@event);
        }
    }

    [Benchmark]
    public void ProcessEvent()
    {
        var i = index++;
        if (i >= events.Length)
        {
            index = 1;
            i = 0;
        }

        service.ProcessEvent(events[i]);
    }
}
