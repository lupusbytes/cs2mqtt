using System.Net;
using System.Text;
using Microsoft.Extensions.Logging.Abstractions;
using MQTTnet;
using MQTTnet.Protocol;
using MQTTnet.Server;
using IMqttNetClient = MQTTnet.IMqttClient;

namespace LupusBytes.CS2.GameStateIntegration.Mqtt.Integration.Tests;

public class MqttClientTest
{
    private const string ListenAddress = "127.0.0.1";
    private const int ListenPort = 1883;
    private const int TimeoutMilliseconds = 1000;

    [Fact]
    public async Task Last_will_and_testament()
    {
        // Arrange
        var clientFactory = new MqttClientFactory();
        var serverFactory = new MqttServerFactory();

        using var testServer = await CreateTestServerAsync(serverFactory);
        using var testClient = await CreateTestClientAsync(clientFactory);

        var tcs = new TaskCompletionSource<string>();

        await SubscribeToTopicAsync(
            testClient,
            MqttConstants.SystemAvailabilityTopic,
            message => tcs.TrySetResult(message));

        var options = new MqttOptions
        {
            Host = ListenAddress,
            Port = ListenPort,
            UseTls = false,
        };

        var optionsProvider = Substitute.For<IMqttOptionsProvider>();
        optionsProvider.GetOptionsAsync(Arg.Any<CancellationToken>()).Returns(options);

        using var sut = new MqttClient(
            clientFactory.CreateMqttClient(),
            optionsProvider,
            onFatalConnectionError: () => Task.CompletedTask,
            NullLogger<MqttClient>.Instance);

        await sut.StartAsync(CancellationToken.None);

        // Act
        await testServer.DisconnectClientAsync(options.ClientId); // Kicks the sut

        // Assert
        await AssertPayload("offline", tcs);
    }

    [Theory]
    [InlineData("cs2mqtt/test", "Hello World")]
    [InlineData("lupusbytes/cs2mqtt", "Foo bar")]
    public async Task PublishAsync_publishes_message(string topic, string payload)
    {
        // Arrange
        var clientFactory = new MqttClientFactory();
        var serverFactory = new MqttServerFactory();

        using var testServer = await CreateTestServerAsync(serverFactory);
        using var testClient = await CreateTestClientAsync(clientFactory);

        var tcs = new TaskCompletionSource<string>();

        await SubscribeToTopicAsync(
            testClient,
            topic,
            message => tcs.TrySetResult(message));

        var optionsProvider = Substitute.For<IMqttOptionsProvider>();
        optionsProvider.GetOptionsAsync(Arg.Any<CancellationToken>()).Returns(new MqttOptions
        {
            Host = ListenAddress,
            Port = ListenPort,
            UseTls = false,
        });

        using var sut = new MqttClient(
            clientFactory.CreateMqttClient(),
            optionsProvider,
            onFatalConnectionError: () => Task.CompletedTask,
            NullLogger<MqttClient>.Instance);

        await sut.StartAsync(CancellationToken.None);

        // Act
        await sut.PublishAsync(new MqttMessage(topic, payload), CancellationToken.None);

        // Assert
        await AssertPayload(payload, tcs);
    }

    [Theory, AutoData]
    public async Task Connects_with_credentials(string username, string password)
    {
        // Arrange
        using var testServer = await CreateTestServerWithAuthenticationAsync(
            new MqttServerFactory(),
            username,
            password);

        var optionsProvider = Substitute.For<IMqttOptionsProvider>();
        optionsProvider.GetOptionsAsync(Arg.Any<CancellationToken>()).Returns(new MqttOptions
        {
            Host = ListenAddress,
            Port = ListenPort,
            Username = username,
            Password = password,
            UseTls = false,
            RetryDelayProvider = _ => TimeSpan.Zero,
        });

        var fatalErrorCalled = false;

        using var sut = new MqttClient(
            new MqttClientFactory().CreateMqttClient(),
            optionsProvider,
            onFatalConnectionError: () =>
            {
                fatalErrorCalled = true;
                return Task.CompletedTask;
            },
            NullLogger<MqttClient>.Instance);

        // Act
        await sut.StartAsync(CancellationToken.None);

        // Assert
        sut.IsConnected.Should().BeTrue();
        fatalErrorCalled.Should().BeFalse();
    }

    private static async Task AssertPayload(string expected, TaskCompletionSource<string> tcs)
    {
        // Await which task completes first. Our task completion source or a timeout
        var completedTask = await Task.WhenAny(tcs.Task, Task.Delay(TimeoutMilliseconds));

        // The start task completion source should have finished first.
        completedTask.Should().Be(
            tcs.Task,
            $"Expected to receive a message within the timeout period ({TimeoutMilliseconds} miliseconds)");

        // Finally assert that the received message is equal to the expected message.
        (await tcs.Task).Should().Be(expected);
    }

    private static async Task<IMqttNetClient> CreateTestClientAsync(MqttClientFactory factory)
    {
        var client = factory.CreateMqttClient();
        var clientOptions = new MqttClientOptionsBuilder()
            .WithTcpServer(ListenAddress, ListenPort)
            .Build();

        await client.ConnectAsync(clientOptions);
        return client;
    }

    private static async Task<MqttServer> CreateTestServerAsync(MqttServerFactory factory)
    {
        var serverOptions = new MqttServerOptionsBuilder()
            .WithDefaultEndpoint()
            .WithDefaultEndpointBoundIPAddress(IPAddress.Parse(ListenAddress))
            .WithDefaultEndpointPort(ListenPort)
            .Build();

        var server = factory.CreateMqttServer(serverOptions);
        await server.StartAsync();
        return server;
    }

    private static async Task<MqttServer> CreateTestServerWithAuthenticationAsync(
        MqttServerFactory factory,
        string username,
        string password)
    {
        var server = await CreateTestServerAsync(factory);
        server.ValidatingConnectionAsync += e =>
        {
            if (e.UserName != username || e.Password != password)
            {
                e.ReasonCode = MqttConnectReasonCode.NotAuthorized;
            }

            return Task.CompletedTask;
        };

        return server;
    }

    private static Task<MqttClientSubscribeResult> SubscribeToTopicAsync(
        IMqttNetClient client,
        string topic,
        Action<string> onMessageReceived)
    {
        var subscribeOptions = new MqttClientSubscribeOptionsBuilder()
            .WithTopicFilter(topic)
            .Build();

        client.ApplicationMessageReceivedAsync += e =>
        {
            onMessageReceived(Encoding.UTF8.GetString(e.ApplicationMessage.Payload));
            return Task.CompletedTask;
        };

        return client.SubscribeAsync(subscribeOptions);
    }
}