using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Threading.Channels;
using LupusBytes.CS2.GameStateIntegration.Events;
using Microsoft.Extensions.Hosting;
using MQTTnet;
using MQTTnet.Client;

namespace LupusBytes.CS2.GameStateIntegration.Mqtt;

public sealed class GameStateMqttPublisher(
    GameStateService gameStateService,
    MqttOptions options)
    : BackgroundService, IGameStateObserver
{
    private const string BaseTopic = "cs2mqtt";

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
                await mqttClient.PublishAsync(CreateEventMessage(@event), stoppingToken);
            }
        }
    }

    private static MqttApplicationMessage CreateEventMessage(BaseEvent @event)
    {
        string topic;
        string payload;
        switch (@event)
        {
            case MapEvent m:
                topic = $"{BaseTopic}/{@event.SteamId}/round";
                payload = JsonSerializer.Serialize(m.Map);
                break;
            case PlayerEvent p:
                topic = $"{BaseTopic}/{@event.SteamId}/round";
                payload = JsonSerializer.Serialize(p.Player);
                break;
            case RoundEvent r:
                topic = $"{BaseTopic}/{@event.SteamId}/round";
                payload = JsonSerializer.Serialize(r.Round);
                break;
            default: throw new SwitchExpressionException();
        }

        return new MqttApplicationMessageBuilder()
            .WithTopic(topic)
            .WithPayload(payload)
            .Build();
    }

    void IObserver<MapEvent>.OnCompleted()
    {
    }

    void IObserver<RoundEvent>.OnCompleted()
    {
    }

    void IObserver<PlayerEvent>.OnCompleted()
    {
    }

    void IObserver<RoundEvent>.OnError(Exception error)
    {
    }

    void IObserver<PlayerEvent>.OnError(Exception error)
    {
    }

    void IObserver<MapEvent>.OnError(Exception error)
    {
    }
}