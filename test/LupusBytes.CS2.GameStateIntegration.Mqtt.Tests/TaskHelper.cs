namespace LupusBytes.CS2.GameStateIntegration.Mqtt.Tests;

public static class TaskHelper
{
    public static TaskCompletionSource<bool> CompletionSourceFromTopicPublishment(
        IMqttClient mqttClient,
        string topic)
    {
        var tcs = new TaskCompletionSource<bool>();
        mqttClient
            .When(x => x.PublishAsync(
                Arg.Is<MqttMessage>(m => m.Topic == topic),
                Arg.Any<CancellationToken>()))
            .Do(_ => tcs.SetResult(true));
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