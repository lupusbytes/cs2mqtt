using System.Collections.Concurrent;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MQTTnet;
using MQTTnet.Client;
using Polly;
using IMqttNetClient = MQTTnet.Client.IMqttClient;

namespace LupusBytes.CS2.GameStateIntegration.Mqtt;

public sealed class MqttClient : IHostedService, IMqttClient, IDisposable
{
    private const int ConnectRetryCount = 3;
    private const int ReconnectRetryCount = 5;

    private readonly MqttOptions options;
    private readonly MqttClientOptions clientOptions;
    private readonly ILogger<MqttClient> logger;
    private readonly IMqttNetClient mqttNetClient;

    private ConcurrentDictionary<string, MqttMessage> backlog = new(StringComparer.Ordinal);
    private bool shutdownRequested;

    public MqttClient(IMqttNetClient mqttNetClient, MqttOptions options, ILogger<MqttClient> logger)
    {
        this.mqttNetClient = mqttNetClient;
        this.mqttNetClient.ConnectedAsync += OnConnected;
        this.mqttNetClient.DisconnectedAsync += OnDisconnected;
        this.options = options;
        this.logger = logger;

        var clientOptionsBuilder = new MqttClientOptionsBuilder()
            .WithTcpServer(options.Host, options.Port)
            .WithClientId(options.ClientId)
            .WithTlsOptions(b => b.UseTls(options.UseTls))
            .WithWillTopic(MqttConstants.SystemAvailabilityTopic)
            .WithWillPayload("offline")
            .WithWillRetain();

        if (!string.IsNullOrWhiteSpace(options.Username))
        {
            clientOptionsBuilder.WithCredentials(options.Username, options.Password);
        }

        clientOptions = clientOptionsBuilder.Build();
    }

    private Task ConnectAsync(int retryCount, CancellationToken cancellationToken = default) => Policy
        .Handle<Exception>()
        .WaitAndRetryAsync(
            retryCount,
            sleepDurationProvider: attempt => TimeSpan.FromSeconds(Math.Pow(2.5, attempt)),
            onRetry: (_, timeSpan, currentAttempt, _)
                => logger.ConnectionToMqttBrokerFailed(
                    options.Host,
                    options.Port,
                    currentAttempt,
                    retryCount + 1,
                    timeSpan.Seconds))
        .ExecuteAsync(
            async token =>
            {
                logger.ConnectingToMqttBroker(options.Host, options.Port);
                await mqttNetClient.ConnectAsync(clientOptions, token);
            },
            cancellationToken);

    private Task OnConnected(MqttClientConnectedEventArgs arg)
    {
        logger.ConnectedToMqttBroker(options.Host, options.Port);
        return PublishBacklogAsync();
    }

    private Task PublishBacklogAsync()
    {
        var messages = backlog.Values;
        backlog = new ConcurrentDictionary<string, MqttMessage>(StringComparer.Ordinal);

        return Task.WhenAll(messages.Select(x => PublishAsync(x, CancellationToken.None)));
    }

    private Task OnDisconnected(MqttClientDisconnectedEventArgs args)
    {
        if (!args.ClientWasConnected)
        {
            return Task.CompletedTask;
        }

        logger.DisconnectedFromBroker(options.Host, options.Port);

        return !shutdownRequested
            ? ConnectAsync(ReconnectRetryCount, CancellationToken.None)
            : Task.CompletedTask;
    }

    public async Task PublishAsync(MqttMessage message, CancellationToken cancellationToken)
    {
        if (!mqttNetClient.IsConnected)
        {
            backlog[message.Topic] = message;
            return;
        }

        var mqttMessage = new MqttApplicationMessageBuilder()
            .WithTopic(message.Topic)
            .WithPayload(message.Payload)
            .WithRetainFlag(message.RetainFlag)
            .Build();

        await mqttNetClient.PublishAsync(mqttMessage, cancellationToken);
        logger.MqttMessagePublished(message.Topic, message.Payload);
    }

    public void Dispose() => mqttNetClient.Dispose();

    public Task StartAsync(CancellationToken cancellationToken)
    {
        shutdownRequested = false;
        return ConnectAsync(ConnectRetryCount, cancellationToken);
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        shutdownRequested = true;

        await PublishAsync(
            new MqttMessage(
                MqttConstants.SystemAvailabilityTopic,
                "offline",
                RetainFlag: true),
            cancellationToken);

        await mqttNetClient.DisconnectAsync(cancellationToken: cancellationToken);
    }
}