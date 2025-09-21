using System.Diagnostics.CodeAnalysis;
using System.Net.Sockets;
using System.Text;
using Microsoft.Extensions.Logging;
using MQTTnet;
using MQTTnet.Adapter;
using NSubstitute.ExceptionExtensions;
using IMqttNetClient = MQTTnet.IMqttClient;

namespace LupusBytes.CS2.GameStateIntegration.Mqtt.Tests;

public class MqttClientTest
{
    [Theory]
    [InlineAutoNSubstituteData("5")]
    [InlineAutoNSubstituteData("4.0.0")]
    [InlineAutoNSubstituteData("Foo")]
    public void Constructor_throws_on_invalid_MQTT_protocol(
        string protocolVersion,
        IMqttNetClient mqttNetClient,
        ILogger<MqttClient> logger)
    {
        // Arrange
        var options = new MqttOptions
        {
            ProtocolVersion = protocolVersion,
        };

        // Act & Assert
        Assert.Throws<ArgumentException>(() => new MqttClient(mqttNetClient, options, logger));
    }

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
                Encoding.UTF8.GetString(x.Payload) == message.Payload &&
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
        // Arrange
        ArrangeConnectResultCode(mqttNetClient, MqttClientConnectResultCode.Success);

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
    public async Task StartAsync_retries_and_succeeds_after_initial_failure(
        [Frozen] IMqttNetClient mqttNetClient,
        [Frozen] MqttOptions options,
        MqttClient sut)
    {
        // Arrange
        ArrangeConnectResultCode(
            mqttNetClient,
            MqttClientConnectResultCode.UnspecifiedError,
            MqttClientConnectResultCode.Success);

        options.ConnectRetryCount = 1;
        options.RetryDelayProvider = _ => TimeSpan.Zero;

        // Act
        await sut.StartAsync(CancellationToken.None);

        // Assert
        await mqttNetClient.Received(options.ConnectRetryCount + 1).ConnectAsync(
            Arg.Any<MqttClientOptions>(),
            Arg.Any<CancellationToken>());
    }

    [Theory]
    [MemberAutoNSubstituteData(nameof(AllFailureCodes))]
    public async Task StartAsync_throws_if_connect_result_not_success(
        MqttClientConnectResultCode resultCode,
        [Frozen] IMqttNetClient mqttNetClient,
        [Frozen] MqttOptions options,
        MqttClient sut)
    {
        // Arrange
        ArrangeConnectResultCode(mqttNetClient, resultCode);
        options.RetryDelayProvider = _ => TimeSpan.Zero;
        options.ConnectRetryCount = 3;

        // Act & Assert
        await Assert.ThrowsAsync<MqttConnectingFailedException>(
            () => sut.StartAsync(CancellationToken.None));

        await mqttNetClient.Received(options.ConnectRetryCount + 1).ConnectAsync(
            Arg.Any<MqttClientOptions>(),
            Arg.Any<CancellationToken>());
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
        ArrangeConnectResultCode(mqttNetClient, MqttClientConnectResultCode.Success);
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
            Arg.Is<MqttApplicationMessage>(x => Encoding.UTF8.GetString(x.Payload) == "Hello World"),
            Arg.Any<CancellationToken>());
    }

    [Theory, AutoNSubstituteData]
    [SuppressMessage("Usage", "xUnit1026:Theory methods should use all of their parameters", Justification = "sut's event handler is doing work behind the scenes")]
    [SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "sut's event handler is doing work behind the scenes")]
    public Task Attempts_to_reconnect_after_disconnect_and_throws_after_exhausting_retries(
        [Frozen] IMqttNetClient mqttNetClient,
        [Frozen] MqttOptions mqttOptions,
        MqttClient sut)
    {
        // Arrange
        mqttOptions.ReconnectRetryCount = 1;
        mqttOptions.RetryDelayProvider = _ => TimeSpan.Zero;
        mqttNetClient.IsConnected.Returns(false);
        mqttNetClient
            .ConnectAsync(Arg.Any<MqttClientOptions>(), Arg.Any<CancellationToken>())
            .ThrowsAsync<TimeoutException>();

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
        return mqttNetClient
            .Received(mqttOptions.ReconnectRetryCount + 1)
            .ConnectAsync(Arg.Any<MqttClientOptions>(), Arg.Any<CancellationToken>());
    }

    [Theory]
    [InlineAutoNSubstituteData(-1)]
    [InlineAutoNSubstituteData(0)]
    [InlineAutoNSubstituteData(1)]
    [InlineAutoNSubstituteData(10)]
    [SuppressMessage("Usage", "xUnit1026:Theory methods should use all of their parameters", Justification = "sut's event handler is doing work behind the scenes")]
    [SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "sut's event handler is doing work behind the scenes")]
    public Task Attempts_to_reconnect_after_disconnect_and_succeeds(
        int reconnectRetryCount,
        [Frozen] IMqttNetClient mqttNetClient,
        [Frozen] MqttOptions mqttOptions,
        MqttClient sut)
    {
        // Arrange
        var resultCodes = Enumerable
            .Range(0, Math.Max(0, reconnectRetryCount))
            .Select(_ => MqttClientConnectResultCode.UnspecifiedError)
            .Concat([MqttClientConnectResultCode.Success])
            .ToArray();

        ArrangeConnectResultCode(mqttNetClient, resultCodes);

        mqttOptions.ReconnectRetryCount = reconnectRetryCount;
        mqttOptions.RetryDelayProvider = _ => TimeSpan.Zero;
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
        return mqttNetClient
            .Received(resultCodes.Length)
            .ConnectAsync(Arg.Any<MqttClientOptions>(), Arg.Any<CancellationToken>());
    }

    [Theory, AutoNSubstituteData]
    [SuppressMessage("Usage", "xUnit1026:Theory methods should use all of their parameters", Justification = "sut's event handler is doing work behind the scenes")]
    [SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "sut's event handler is doing work behind the scenes")]
    public Task Does_not_attempt_to_reconnect_more_than_once_if_ReconnectRetryCount_options_is_0(
        [Frozen] IMqttNetClient mqttNetClient,
        [Frozen] MqttOptions mqttOptions,
        MqttClient sut)
    {
        // Arrange
        ArrangeConnectResultCode(mqttNetClient, MqttClientConnectResultCode.UnspecifiedError);

        mqttOptions.ReconnectRetryCount = 0;
        mqttOptions.RetryDelayProvider = _ => TimeSpan.Zero;
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

    [Theory, AutoNSubstituteData]
    [SuppressMessage("Usage", "xUnit1026:Theory methods should use all of their parameters", Justification = "sut's event handler is doing work behind the scenes")]
    [SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "sut's event handler is doing work behind the scenes")]
    public Task MqttClientDisconnectedEvent_does_nothing_if_client_was_not_connected(
        [Frozen] IMqttNetClient mqttNetClient,
        MqttClient sut)
    {
        // Arrange
        var eventArgs = new MqttClientDisconnectedEventArgs(
            clientWasConnected: false,
            new MqttClientConnectResult(),
            MqttClientDisconnectReason.UnspecifiedError,
            reasonString: "bar",
            [],
            new SocketException());

        // Act
        mqttNetClient.DisconnectedAsync += Raise.Event<Func<MqttClientDisconnectedEventArgs, Task>>(eventArgs);

        // Assert
        return mqttNetClient.DidNotReceive().ConnectAsync(Arg.Any<MqttClientOptions>(), Arg.Any<CancellationToken>());
    }

    public static TheoryData<MqttClientConnectResultCode> AllFailureCodes
        => new(Enum.GetValues<MqttClientConnectResultCode>().Where(c => c != MqttClientConnectResultCode.Success));

    private static void ArrangeConnectResultCode(IMqttNetClient mqttNetClient, params IEnumerable<MqttClientConnectResultCode> codes)
    {
        var results = codes.Select(CreateConnectResult).ToArray();

        mqttNetClient
            .ConnectAsync(Arg.Any<MqttClientOptions>(), Arg.Any<CancellationToken>())
            .Returns(results[0], results[1..]);
    }

    /// <summary>
    /// IMqttNetClient.ConnectAsync returns a MqttClientConnectResult,
    /// which our system under test uses to validate that the connection is successfully established.
    /// This result object only has internal setters for all the properties, so this helper method
    /// can be used to leverage reflection to create a MqttClientConnectResult with the desired ResultCode.
    /// </summary>
    private static MqttClientConnectResult CreateConnectResult(MqttClientConnectResultCode code)
    {
        var result = new MqttClientConnectResult();

        typeof(MqttClientConnectResult)
            .GetProperty(nameof(MqttClientConnectResult.ResultCode))!
            .SetValue(result, code);

        return result;
    }
}