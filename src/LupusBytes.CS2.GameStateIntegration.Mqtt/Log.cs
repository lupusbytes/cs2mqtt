using Microsoft.Extensions.Logging;

namespace LupusBytes.CS2.GameStateIntegration.Mqtt;

/// <summary>
/// Class for compile-time logging source generation
/// </summary>
public static partial class Log
{
    [LoggerMessage(
        EventId = 1_00,
        Level = LogLevel.Information,
        Message = "MQTT publish: topic '{Topic}', payload '{Payload}'")]
    public static partial void MqttMessagePublished(this ILogger logger, string topic, string payload);

    [LoggerMessage(
        EventId = 1_01,
        Level = LogLevel.Information,
        Message = "Connecting to MQTT broker {Host}:{Port}")]
    public static partial void ConnectingToMqttBroker(this ILogger logger, string host, int port);

    [LoggerMessage(
        EventId = 1_02,
        Level = LogLevel.Information,
        Message = "Successfully connected to MQTT broker {Host}:{Port}")]
    public static partial void ConnectedToMqttBroker(this ILogger logger, string host, int port);

    [LoggerMessage(
        EventId = 1_03,
        Level = LogLevel.Error,
        Message = "Error connecting to MQTT broker {Host}:{Port}. This was attempt {RetryCount} of {TotalRetryCount}. Waiting {waitTime} seconds before retrying")]
    public static partial void ConnectionToMqttBrokerFailed(this ILogger logger, string host, int port, int retryCount, int totalRetryCount, int waitTime);

    [LoggerMessage(
        EventId = 1_04,
        Level = LogLevel.Error,
        Message = "Disconnected from MQTT broker {Host}:{Port}")]
    public static partial void DisconnectedFromBroker(this ILogger logger, string host, int port);
}