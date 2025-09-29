using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace LupusBytes.CS2.GameStateIntegration.Mqtt.HomeAssistant;

/// <summary>
/// Home Assistant Supervisor API response contract.
/// </summary>
/// <typeparam name="T">The type of the data payload returned by the Supervisor.</typeparam>
/// <param name="Result">The response status. Either "ok" or "error".</param>
/// <param name="Message">Message returned in case of an erroneous result</param>
/// <param name="Data">T — The returned data.</param>
public record SupervisorResponse<T>(
    [property: ExcludeFromCodeCoverage, JsonPropertyName("result")] string Result,
    [property: ExcludeFromCodeCoverage, JsonPropertyName("message")] string? Message,
    [property: JsonPropertyName("data")] T? Data);