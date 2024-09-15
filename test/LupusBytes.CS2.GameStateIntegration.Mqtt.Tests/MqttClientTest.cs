using System.Diagnostics.CodeAnalysis;
using System.Net.Sockets;
using System.Text;
using MQTTnet;
using MQTTnet.Client;
using IMqttNetClient = MQTTnet.Client.IMqttClient;

namespace LupusBytes.CS2.GameStateIntegration.Mqtt.Tests;

public class MqttClientTest
{
    [Theory, AutoNSubstituteData]
    public async Task PublishAsync_invokes_MQTTnet_PublishAsync(
        [Frozen] IMqttNetClient mqttNetClient,
        MqttMessage message,
        CancellationToken cancellationToken,
        MqttClient sut)
    {
        // Arrange
        mqttNetClient.IsConnected.Returns(true);

        // Act
        await sut.PublishAsync(message, cancellationToken);

        // Assert
        await mqttNetClient.Received(1).PublishAsync(
            Arg.Is<MqttApplicationMessage>(x =>
                x.Topic == message.Topic &&
                Encoding.UTF8.GetString(x.PayloadSegment.Array!, x.PayloadSegment.Offset, x.PayloadSegment.Count) == message.Payload &&
                x.Retain == message.RetainFlag),
            Arg.Is(cancellationToken));
    }

    [Theory, AutoNSubstituteData]
    public async Task StartAsync_invokes_MQTTnet_ConnectAsync(
        [Frozen] IMqttNetClient mqttNetClient,
        [Frozen] MqttOptions options,
        CancellationToken cancellationToken,
        MqttClient sut)
    {
        // Act
        await sut.StartAsync(cancellationToken);

        // Assert
        await mqttNetClient.Received(1).ConnectAsync(
            Arg.Is<MqttClientOptions>(x =>
                x.ClientId == options.ClientId &&
                x.ChannelOptions is MqttClientTcpOptions &&
                ((MqttClientTcpOptions)x.ChannelOptions).RemoteEndpoint.ToString() == $"Unspecified/{options.Host}:{options.Port}" &&
                x.ChannelOptions.TlsOptions.UseTls == options.UseTls),
            Arg.Is(cancellationToken));
    }

    [Theory, AutoNSubstituteData]
    public async Task StopAsync_invokes_MQTTnet_DisconnectAsync(
        [Frozen] IMqttNetClient mqttNetClient,
        CancellationToken cancellationToken,
        MqttClient sut)
    {
        // Act
        await sut.StopAsync(cancellationToken);

        // Assert
        await mqttNetClient.Received(1).DisconnectAsync(Arg.Any<MqttClientDisconnectOptions>(), Arg.Is(cancellationToken));
    }

    [Theory, AutoNSubstituteData]
    public async Task StopAsync_does_not_trigger_reconnect_attempt(
        [Frozen] IMqttNetClient mqttNetClient,
        CancellationToken cancellationToken,
        MqttClient sut)
    {
        // Arrange
        await sut.StartAsync(cancellationToken);
        mqttNetClient.IsConnected.Returns(false);

        // Act
        await sut.StopAsync(cancellationToken);
        mqttNetClient.DisconnectedAsync += Raise.Event<Func<MqttClientDisconnectedEventArgs, Task>>(
            new MqttClientDisconnectedEventArgs(
                clientWasConnected: true,
                new MqttClientConnectResult(),
                MqttClientDisconnectReason.UnspecifiedError,
                reasonString: "foo",
                [],
                new SocketException()));

        // Assert that the MQTTnet client only received 1 connect call - when it was starting.
        await mqttNetClient.Received(1).ConnectAsync(Arg.Any<MqttClientOptions>(), Arg.Any<CancellationToken>());
    }

    [Theory, AutoNSubstituteData]
    public void Dispose_invokes_MQTTnet_Dispose(
        [Frozen] IMqttNetClient mqttNetClient,
        MqttClient sut)
    {
        // Act
        sut.Dispose();

        // Assert
        mqttNetClient.Received(1).Dispose();
    }

    [Theory, AutoNSubstituteData]
    public async Task Publishes_last_message_collected_for_each_topic_while_disconnected_on_reconnect(
        [Frozen] IMqttNetClient mqttNetClient,
        IReadOnlyCollection<MqttMessage> messages,
        MqttClient sut)
    {
        // Arrange
        mqttNetClient.IsConnected.Returns(false);
        foreach (var message in messages)
        {
            // Publish messages
            await sut.PublishAsync(message, default);
        }

        foreach (var message in messages.Select(message => message with { Payload = "Hello World" }))
        {
            // Publish messages on the same topics, but with Hello World as payload
            await sut.PublishAsync(message, default);
        }

        // Act
        mqttNetClient.IsConnected.Returns(true);
        mqttNetClient.ConnectedAsync += Raise.Event<Func<MqttClientConnectedEventArgs, Task>>(new MqttClientConnectedEventArgs(new MqttClientConnectResult()));

        // Assert
        await mqttNetClient.Received(messages.Count).PublishAsync(
            Arg.Is<MqttApplicationMessage>(x =>
                Encoding.UTF8.GetString(
                    x.PayloadSegment.Array!,
                    x.PayloadSegment.Offset,
                    x.PayloadSegment.Count) == "Hello World"),
            Arg.Any<CancellationToken>());
    }

    [Theory, AutoNSubstituteData]
    [SuppressMessage("Usage", "xUnit1026:Theory methods should use all of their parameters", Justification = "sut's event handler is doing work behind the scenes")]
    [SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "sut's event handler is doing work behind the scenes")]
    public Task Attempts_to_reconnect_after_disconnect(
        [Frozen] IMqttNetClient mqttNetClient,
        MqttClient sut)
    {
        // Arrange
        mqttNetClient.IsConnected.Returns(false);

        // Act
        mqttNetClient.DisconnectedAsync += Raise.Event<Func<MqttClientDisconnectedEventArgs, Task>>(
            new MqttClientDisconnectedEventArgs(
                clientWasConnected: true,
                new MqttClientConnectResult(),
                MqttClientDisconnectReason.UnspecifiedError,
                reasonString: "foo",
                [],
                new SocketException()));

        // Assert
        return mqttNetClient.Received(1).ConnectAsync(Arg.Any<MqttClientOptions>(), Arg.Any<CancellationToken>());
    }
}