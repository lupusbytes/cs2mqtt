namespace LupusBytes.CS2.GameStateIntegration.Mqtt.Tests;

public class AvailabilityMqttPublisherTests
{
    [Theory, AutoNSubstituteData]
    public async Task Publishes_system_availability_on_startup(
        [Frozen] IMqttClient mqttClient,
        AvailabilityMqttPublisher sut)
    {
        // Act
        await sut.StartAsync(CancellationToken.None);

        // Assert
        await mqttClient.Received(1).PublishAsync(
            Arg.Is<MqttMessage>(x =>
                x.Topic == MqttConstants.SystemAvailabilityTopic &&
                x.Payload == "online" &&
                x.RetainFlag),
            Arg.Any<CancellationToken>());
    }
}