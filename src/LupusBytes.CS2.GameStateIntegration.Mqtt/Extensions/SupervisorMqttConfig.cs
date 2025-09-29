using System.Text.Json.Serialization;

namespace LupusBytes.CS2.GameStateIntegration.Mqtt.Extensions;

/// <summary>
/// Supervisor GET response for <code>/services/mqtt</code> endpoint.
/// Contains MQTT connection info and credentials.
/// </summary>
/// <param name="Addon">The addon providing MQTT functionality.</param>
/// <param name="Host">The IP of the addon running the service.</param>
/// <param name="Port">The port the service is running on.</param>
/// <param name="SSL">True if SSL is in use.</param>
/// <param name="Username">The username for the service.</param>
/// <param name="Password">The password for the service.</param>
/// <param name="Protocol">The MQTT protocol version.</param>
public record SupervisorMqttConfig(
    [property: JsonPropertyName("addon")] string Addon,
    [property: JsonPropertyName("host")] string Host,
    [property: JsonPropertyName("port")] int Port,
    [property: JsonPropertyName("ssl")] bool SSL,
    [property: JsonPropertyName("Username")] string? Username,
    [property: JsonPropertyName("Password")] string? Password,
    [property: JsonPropertyName("Protocol")] string? Protocol);