using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MQTTnet;
using MQTTnet.Adapter;
using MQTTnet.Formatter;
using IMqttNetClient = MQTTnet.IMqttClient;

namespace LupusBytes.CS2.GameStateIntegration.Mqtt;

public sealed class MqttClient : IHostedService, IMqttClient, IDisposable
{
    private readonly MqttOptions options;
    private readonly Func<Task> onFatalConnectionError;
    private readonly MqttClientOptions clientOptions;
    private readonly ILogger<MqttClient> logger;
    private readonly IMqttNetClient mqttNetClient;

    private ConcurrentDictionary<string, MqttMessage> backlog = new(StringComparer.Ordinal);
    private bool shutdownRequested;

    public MqttClient(
        IMqttNetClient mqttNetClient,
        MqttOptions options,
        Func<Task> onFatalConnectionError,
        ILogger<MqttClient> logger)
    {
        this.mqttNetClient = mqttNetClient;
        this.mqttNetClient.ConnectedAsync += OnConnected;
        this.mqttNetClient.DisconnectedAsync += OnDisconnected;
        this.onFatalConnectionError = onFatalConnectionError;
        this.options = options;
        this.logger = logger;

        var clientOptionsBuilder = new MqttClientOptionsBuilder()
            .WithTcpServer(options.Host, options.Port)
            .WithClientId(options.ClientId)
            .WithProtocolVersion(ConvertProtocolVersion(options.ProtocolVersion))
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

    public bool IsConnected => mqttNetClient.IsConnected;

    public Task StartAsync(CancellationToken cancellationToken)
    {
        shutdownRequested = false;
        return ConnectAsync(options.ConnectRetryCount, cancellationToken);
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        shutdownRequested = true;
        return mqttNetClient.DisconnectAsync(cancellationToken: cancellationToken);
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

    [SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "By design.")]
    private async Task ConnectAsync(int retryCount, CancellationToken cancellationToken = default)
    {
        // Ensure we always attempt to execute the loop at least once, even if the retryCount is negative
        var allowedAttempts = Math.Max(1, retryCount + 1);

        for (var attempt = 1; attempt <= allowedAttempts; attempt++)
        {
            try
            {
                logger.ConnectingToMqttBroker(options.Host, options.Port);

                var result = await mqttNetClient.ConnectAsync(clientOptions, cancellationToken);

                if (result.ResultCode is MqttClientConnectResultCode.Success)
                {
                    return;
                }

                throw new MqttConnectingFailedException(
                    $"Failed to connect to MQTT broker {options.Host}:{options.Port.ToString(CultureInfo.InvariantCulture)}. " +
                    $"Result code: {result.ResultCode}",
                    innerException: null);
            }
            catch (Exception) when (attempt < allowedAttempts)
            {
                var delay = options.RetryDelayProvider(attempt);
                logger.ConnectionToMqttBrokerFailed(
                    options.Host,
                    options.Port,
                    attempt,
                    allowedAttempts,
                    Convert.ToInt32(delay.TotalSeconds));

                await Task.Delay(delay, cancellationToken);
            }
            catch (Exception ex)
            {
                logger.ConnectionToMqttBrokerAttemptsExhausted(
                    ex,
                    options.Host,
                    options.Port,
                    attempt);

                await onFatalConnectionError();
            }
        }
    }

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
            ? ConnectAsync(options.ReconnectRetryCount)
            : Task.CompletedTask;
    }

    private static MqttProtocolVersion ConvertProtocolVersion(string mqttProtocolVersion)
        => mqttProtocolVersion switch
        {
            "5.0.0" => MqttProtocolVersion.V500,
            "3.1.1" => MqttProtocolVersion.V311,
            "3.1.0" => MqttProtocolVersion.V310,
            _ => throw new ArgumentException(
                $"Unknown or unsupported MQTT protocol version: {mqttProtocolVersion}",
                nameof(mqttProtocolVersion)),
        };
}