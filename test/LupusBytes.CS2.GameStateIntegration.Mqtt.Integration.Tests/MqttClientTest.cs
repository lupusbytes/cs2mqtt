using System.Net;
using System.Text;
using Microsoft.Extensions.Logging.Abstractions;
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Server;

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
        using var testServer = await CreateTestServerAsync();
        using var testClient = await CreateTestClientAsync();

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
        using var sut = new MqttClient(options, NullLogger<MqttClient>.Instance);

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
        using var testServer = await CreateTestServerAsync();
        using var testClient = await CreateTestClientAsync();

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
        using var sut = new MqttClient(options, NullLogger<MqttClient>.Instance);

        // Act
        await sut.PublishAsync(new MqttMessage(topic, payload), CancellationToken.None);

        // Assert
        await AssertPayload(payload, tcs);
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

    private static async Task<MQTTnet.Client.IMqttClient> CreateTestClientAsync()
    {
        var factory = new MqttFactory();
        var client = factory.CreateMqttClient();
        var clientOptions = new MqttClientOptionsBuilder()
            .WithTcpServer(ListenAddress, ListenPort)
            .Build();

        await client.ConnectAsync(clientOptions);
        return client;
    }

    private static async Task<MqttServer> CreateTestServerAsync()
    {
        var factory = new MqttFactory();
        var serverOptions = new MqttServerOptionsBuilder()
            .WithDefaultEndpoint()
            .WithDefaultEndpointBoundIPAddress(IPAddress.Parse(ListenAddress))
            .WithDefaultEndpointPort(ListenPort)
            .Build();

        var server = factory.CreateMqttServer(serverOptions);
        await server.StartAsync();
        return server;
    }

    private static Task<MqttClientSubscribeResult> SubscribeToTopicAsync(
        MQTTnet.Client.IMqttClient client,
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