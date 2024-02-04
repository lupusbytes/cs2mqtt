using System.Text.Json.Serialization;

namespace LupusBytes.CS2.GameStateIntegration.Contracts.Enums;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum MapPhase
{
    Warmup,
    Live,
    Intermission,
}