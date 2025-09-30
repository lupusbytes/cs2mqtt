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

    [LoggerMessage(
        EventId = 1_05,
        Level = LogLevel.Critical,
        Message = "Failed to connect to MQTT broker {Host}:{Port} after {Attempts} attempts, giving up!")]
    public static partial void ConnectionToMqttBrokerAttemptsExhausted(this ILogger logger, Exception ex, string host, int port, int attempts);

    [LoggerMessage(
        EventId = 1_06,
        Level = LogLevel.Information,
        Message = "Successfully retrieved HA supervisor token.")]
    public static partial void RetrievedSupervisorToken(this ILogger logger);

    [LoggerMessage(
        EventId = 1_07,
        Level = LogLevel.Information,
        Message = "No supervisor token found.")]
    public static partial void NoSupervisorTokenFound(this ILogger logger);

    [LoggerMessage(
        EventId = 1_08,
        Level = LogLevel.Information,
        Message = "Fetched MQTT info from supervisor: {Host}:{Port}.")]
    public static partial void FetchedMqttInfoFromSupervisor(this ILogger logger, string host, int port);

    [LoggerMessage(
        EventId = 1_09,
        Level = LogLevel.Error,
        Message = "Failed executing HA supervisor API call to fetch MQTT on {Address}. Error: {Error}.")]
    public static partial void SupervisorApiRequestFailed(this ILogger logger, string address, string error);
}