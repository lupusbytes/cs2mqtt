using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MQTTnet;
using MQTTnet.Client;
using Polly;

namespace LupusBytes.CS2.GameStateIntegration.Mqtt;

public sealed class MqttClient : IHostedService, IMqttClient, IDisposable
{
    private const int ReconnectRetryCount = 5;
    private const int ConnectRetryCount = 3;

    private readonly MqttOptions options;
    private readonly MqttClientOptions clientOptions;
    private readonly ILogger<MqttClient> logger;
    private readonly MQTTnet.Client.IMqttClient mqttClient;

    public MqttClient(MqttOptions options, ILogger<MqttClient> logger)
    {
        this.options = options;
        this.logger = logger;
        var factory = new MqttFactory();
        mqttClient = factory.CreateMqttClient();

        // TODO: Implement credentials options
        clientOptions = new MqttClientOptionsBuilder()
            .WithTcpServer(options.Host, options.Port)
            .WithClientId(options.ClientId)
            .WithTlsOptions(b => b.UseTls(options.UseTls))
            .WithWillTopic(MqttConstants.SystemAvailabilityTopic)
            .WithWillPayload("offline")
            .WithWillRetain()
            .Build();

        mqttClient.ConnectedAsync += OnConnected;
        mqttClient.DisconnectedAsync += OnDisconnected;
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
                await mqttClient.ConnectAsync(clientOptions, token);
            },
            cancellationToken);

    private Task OnConnected(MqttClientConnectedEventArgs arg)
    {
        logger.ConnectedToMqttBroker(options.Host, options.Port);
        return Task.CompletedTask;
    }

    private Task OnDisconnected(MqttClientDisconnectedEventArgs args)
    {
        if (!args.ClientWasConnected)
        {
            return Task.CompletedTask;
        }

        logger.DisconnectedFromBroker(options.Host, options.Port);
        return ConnectAsync(ReconnectRetryCount, CancellationToken.None);
    }

    public async Task PublishAsync(MqttMessage message, CancellationToken cancellationToken)
    {
        var mqttMessage = new MqttApplicationMessageBuilder()
            .WithTopic(message.Topic)
            .WithPayload(message.Payload)
            .WithRetainFlag(message.RetainFlag)
            .Build();

        await mqttClient.PublishAsync(mqttMessage, cancellationToken);
        logger.MqttMessagePublished(message.Topic, message.Payload);
    }

    public void Dispose() => mqttClient.Dispose();

    public Task StartAsync(CancellationToken cancellationToken) => ConnectAsync(ConnectRetryCount, cancellationToken);

    public Task StopAsync(CancellationToken cancellationToken) => mqttClient.DisconnectAsync(cancellationToken: cancellationToken);
}