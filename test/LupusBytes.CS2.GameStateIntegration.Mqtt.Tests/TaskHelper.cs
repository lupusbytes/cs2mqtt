namespace LupusBytes.CS2.GameStateIntegration.Mqtt.Tests;

internal static class TaskHelper
{
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

    public static CancellationTokenSource EnableCompletionSourceTimeout(
        TaskCompletionSource<bool> tcs)
    {
        var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));
        cts.Token.Register(() => tcs.TrySetCanceled(CancellationToken.None));
        return cts;
    }
}