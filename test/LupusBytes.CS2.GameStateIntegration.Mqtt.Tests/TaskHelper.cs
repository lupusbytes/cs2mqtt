namespace LupusBytes.CS2.GameStateIntegration.Mqtt.Tests;

internal static class TaskHelper
{
    private static readonly TimeSpan CompletionTimeout = TimeSpan.FromSeconds(5);

    public static TaskCompletionSource<bool> CompletionSourceFromTopicPublishment(
        IMqttClient mqttClient,
        params IEnumerable<string> topics)
    {
        var pending = topics.ToHashSet(StringComparer.Ordinal);
        var lockObj = new Lock();
        var tcs = new TaskCompletionSource<bool>();

        foreach (var topic in pending)
        {
            mqttClient
                .When(x => x.PublishAsync(
                    Arg.Is<MqttMessage>(m => m.Topic == topic),
                    Arg.Any<CancellationToken>()))
                .Do(_ =>
                {
                    lock (lockObj)
                    {
                        if (pending.Remove(topic) && pending.Count == 0)
                        {
                            tcs.SetResult(true);
                        }
                    }
                });
        }

        return tcs;
    }

    /// <summary>
    /// Awaits the completion source, throwing a <see cref="TimeoutException"/> if it does not
    /// complete within <see cref="CompletionTimeout"/>, and stopping early if the test is cancelled.
    /// </summary>
    /// <param name="tcs">The completion source to await.</param>
    /// <returns>A task that completes once <paramref name="tcs"/> does.</returns>
    public static Task WaitForCompletionAsync(TaskCompletionSource<bool> tcs)
        => tcs.Task.WaitAsync(CompletionTimeout, TestContext.Current.CancellationToken);
}