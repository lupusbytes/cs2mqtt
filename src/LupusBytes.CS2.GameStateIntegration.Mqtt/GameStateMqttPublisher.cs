using System.Threading.Channels;
using LupusBytes.CS2.GameStateIntegration.Events;
using LupusBytes.CS2.GameStateIntegration.Mqtt.Extensions;
using Microsoft.Extensions.Hosting;
using MQTTnet;
using MQTTnet.Client;

namespace LupusBytes.CS2.GameStateIntegration.Mqtt;

public sealed class GameStateMqttPublisher(
    IGameStateService gameStateService,
    MqttOptions options) : BackgroundService,
    IObserver<MapEvent>,
    IObserver<PlayerEvent>,
    IObserver<RoundEvent>
{
    public const string BaseTopic = "cs2mqtt";

    private readonly Channel<BaseEvent> channel = Channel.CreateBounded<BaseEvent>(new BoundedChannelOptions(1000)
    {
        SingleWriter = false,
        SingleReader = true,
        FullMode = BoundedChannelFullMode.DropOldest,
    });

    public void OnNext(MapEvent value) => channel.Writer.TryWrite(value);
    public void OnNext(RoundEvent value) => channel.Writer.TryWrite(value);
    public void OnNext(PlayerEvent value) => channel.Writer.TryWrite(value);

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var playerSubscription = gameStateService.Subscribe(this as IObserver<PlayerEvent>);
        using var roundSubscription = gameStateService.Subscribe(this as IObserver<RoundEvent>);
        using var mapSubscription = gameStateService.Subscribe(this as IObserver<MapEvent>);

        var mqttFactory = new MqttFactory();
        using var mqttClient = mqttFactory.CreateMqttClient();

        var mqttClientOptions = new MqttClientOptionsBuilder()
            .WithTcpServer(options.Host, options.Port)
            .WithTlsOptions(b => b.UseTls(options.UseTls))
            .Build();

        await mqttClient.ConnectAsync(mqttClientOptions, stoppingToken);

        while (await channel.Reader.WaitToReadAsync(stoppingToken))
        {
            while (channel.Reader.TryRead(out var @event))
            {
                await mqttClient.PublishAsync(@event.ToMqttMessage(), stoppingToken);
            }
        }
    }

    public void OnCompleted()
    {
    }

    public void OnError(Exception error)
    {
    }
}