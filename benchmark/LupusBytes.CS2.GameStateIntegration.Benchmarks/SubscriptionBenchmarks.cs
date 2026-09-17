using BenchmarkDotNet.Attributes;

namespace LupusBytes.CS2.GameStateIntegration.Benchmarks;

/// <summary>
/// Measures the cost of attaching and detaching a consumer across all six state types.
/// This happens on startup/shutdown and whenever the service creates or tears down
/// the per-SteamID relay, so it is not a hot path - but it is the part of the API
/// that changes shape the most.
/// </summary>
[MemoryDiagnoser]
public class SubscriptionBenchmarks
{
    private GameStateService service = null!;
    private StateUpdateSink sink = null!;

    [Params(0, 8)]
    public int ExistingSubscriberCount { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        service = new GameStateService(new GameStateOptions
        {
            IgnoreSpectatedPlayers = false,
            TimeoutInSeconds = 60 * 60,
            TimeoutCleanupIntervalInSeconds = 60 * 60,
        });

        for (var i = 0; i < ExistingSubscriberCount; i++)
        {
            new StateUpdateSink().SubscribeTo(service);
        }

        sink = new StateUpdateSink();
    }

    [Benchmark]
    public void SubscribeAndUnsubscribeAllStateTypes()
    {
        sink.SubscribeTo(service);
        sink.UnsubscribeFrom(service);
    }
}
