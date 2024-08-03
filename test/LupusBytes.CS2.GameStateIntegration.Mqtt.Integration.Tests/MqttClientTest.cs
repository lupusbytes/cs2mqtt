using System.Net;
using System.Text;
using Microsoft.Extensions.Logging.Abstractions;
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Protocol;
using MQTTnet.Server;
using IMqttNetClient = MQTTnet.Client.IMqttClient;

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
        var factory = new MqttFactory();
        using var testServer = await CreateTestServerAsync(factory);
        using var testClient = await CreateTestClientAsync(factory);

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
            ClientId = "cs2mqtt",
        };
        using var sut = new MqttClient(factory.CreateMqttClient(), options, NullLogger<MqttClient>.Instance);
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
        var factory = new MqttFactory();
        using var testServer = await CreateTestServerAsync(factory);
        using var testClient = await CreateTestClientAsync(factory);

        var tcs = new TaskCompletionSource<string>();

        await SubscribeToTopicAsync(
            testClient,
            topic,
            message => tcs.TrySetResult(message));

        var options = new MqttOptions
        {
            Host = ListenAddress,
            Port = ListenPort,
            UseTls = false,
            ClientId = "cs2mqtt",
        };
        using var sut = new MqttClient(factory.CreateMqttClient(), options, NullLogger<MqttClient>.Instance);
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
        var factory = new MqttFactory();
        using var testServer = await CreateTestServerWithAuthenticationAsync(factory, username, password);

        var options = new MqttOptions
        {
            Host = ListenAddress,
            Port = ListenPort,
            Username = username,
            Password = password,
            UseTls = false,
        };
        using var sut = new MqttClient(factory.CreateMqttClient(), options, NullLogger<MqttClient>.Instance);

        // Act & Assert
        var act = () => sut.StartAsync(CancellationToken.None);
        await act.Should().NotThrowAsync();
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

    private static async Task<IMqttNetClient> CreateTestClientAsync(MqttFactory factory)
    {
        var client = factory.CreateMqttClient();
        var clientOptions = new MqttClientOptionsBuilder()
            .WithTcpServer(ListenAddress, ListenPort)
            .Build();

        await client.ConnectAsync(clientOptions);
        return client;
    }

    private static async Task<MqttServer> CreateTestServerAsync(MqttFactory factory)
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
        MqttFactory factory,
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
            onMessageReceived(Encoding.UTF8.GetString(e.ApplicationMessage.PayloadSegment));
            return Task.CompletedTask;
        };

        return client.SubscribeAsync(subscribeOptions);
    }
}