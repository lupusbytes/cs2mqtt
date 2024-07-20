using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Logging;
using MQTTnet;
using MQTTnet.Client;

namespace LupusBytes.CS2.GameStateIntegration.Mqtt;

[SuppressMessage("Performance", "CA1848:Use the LoggerMessage delegates", Justification = "TODO")]
[SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "Need to catch all connection exceptions")]
public class MqttClient : IMqttClient
{
    private readonly ILogger<MqttClient> logger;
    private readonly MQTTnet.Client.IMqttClient mqttClient;
    private readonly MqttClientOptions mqttOptions;

    public MqttClient(MqttOptions options, ILogger<MqttClient> logger)
    {
        this.logger = logger;
        var factory = new MqttFactory();
        mqttClient = factory.CreateMqttClient();

        // TODO: Implement clientId and credentials options
        mqttOptions = new MqttClientOptionsBuilder()
            .WithTcpServer(options.Host, options.Port)
            .WithTlsOptions(b => b.UseTls(options.UseTls))
            .Build();

        mqttClient.ConnectedAsync += OnConnected;
        mqttClient.DisconnectedAsync += OnDisconnected;

        // Initialize connection
        ConnectAsync().GetAwaiter().GetResult();
    }

    private async Task ConnectAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            logger.LogInformation("Connecting to MQTT broker...");
            await mqttClient.ConnectAsync(mqttOptions, cancellationToken);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error connecting to MQTT broker. Retrying in 5 seconds...");

            // TODO: Poly
            await Task.Delay(TimeSpan.FromSeconds(5), cancellationToken);
            await ConnectAsync(cancellationToken);
        }
    }

    private Task OnConnected(MqttClientConnectedEventArgs arg)
    {
        logger.LogInformation("Connected to MQTT broker");
        return Task.CompletedTask;
    }

    private async Task OnDisconnected(MqttClientDisconnectedEventArgs args)
    {
        logger.LogWarning("Disconnected from MQTT broker. Reconnecting in 5 seconds");
        await Task.Delay(TimeSpan.FromSeconds(5));
        await ConnectAsync(CancellationToken.None);
    }

    public async Task PublishAsync(MqttMessage message, CancellationToken cancellationToken)
    {
        if (!mqttClient.IsConnected)
        {
            logger.LogWarning("MQTT client is not connected. Cannot publish message");
            return;
        }

        // TODO: support retain flag
        var mqttMessage = new MqttApplicationMessageBuilder()
            .WithTopic(message.Topic)
            .WithPayload(message.Payload)
            .Build();

        await mqttClient.PublishAsync(mqttMessage, cancellationToken);
    }
}