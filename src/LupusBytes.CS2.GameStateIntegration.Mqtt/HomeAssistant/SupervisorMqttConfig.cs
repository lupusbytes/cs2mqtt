using System.Text.Json.Serialization;

namespace LupusBytes.CS2.GameStateIntegration.Mqtt.HomeAssistant;

/// <summary>
/// Home Assistant Supervisor MQTT config API contract for /services/mqtt endpoint.
/// Contains MQTT broker info and credentials.
/// </summary>
/// <param name="Addon">The add-on providing MQTT functionality.</param>
/// <param name="Host">The hostname/address of the service.</param>
/// <param name="Port">The port the service is running on.</param>
/// <param name="Ssl">True if SSL is in use.</param>
/// <param name="Username">The username for the service.</param>
/// <param name="Password">The password for the service.</param>
/// <param name="Protocol">The MQTT protocol version.</param>
public record SupervisorMqttConfig(
    [property: JsonPropertyName("addon")] string Addon,
    [property: JsonPropertyName("host")] string Host,
    [property: JsonPropertyName("port")] int Port,
    [property: JsonPropertyName("ssl")] bool Ssl,
    [property: JsonPropertyName("username")] string Username,
    [property: JsonPropertyName("password")] string Password,
    [property: JsonPropertyName("protocol")] string Protocol);