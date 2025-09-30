using System.Text.Json.Serialization;

namespace LupusBytes.CS2.GameStateIntegration.Mqtt;

/// <summary>
/// Supervisor GET response for <code>/services/mqtt</code> endpoint.
/// Contains response status, MQTT connection info and credentials.
/// </summary>
/// <param name="Result">The response status.</param>
/// <param name="Data">The MQTT config.</param>
public record SupervisorMqttConfigResponse(
    [property: JsonPropertyName("result")] string Result,
    [property: JsonPropertyName("data")] SupervisorMqttConfig? Data);