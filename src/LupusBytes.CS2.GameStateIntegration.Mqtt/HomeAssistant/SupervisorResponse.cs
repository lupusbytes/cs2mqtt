using System.Text.Json.Serialization;

namespace LupusBytes.CS2.GameStateIntegration.Mqtt.HomeAssistant;

/// <summary>
/// Supervisor GET response for <code>/services/mqtt</code> endpoint.
/// Contains response status, MQTT connection info and credentials.
/// </summary>
/// <typeparam name="T">The type of the data payload returned by the Supervisor.</typeparam>
/// <param name="Result">The response status.</param>
/// <param name="Message">Message returned in case of an erroneous result</param>
/// <param name="Data">T — The returned data.</param>
public record SupervisorResponse<T>(
    [property: JsonPropertyName("result")] string Result,
    [property: JsonPropertyName("message")] string Message,
    [property: JsonPropertyName("data")] T Data);