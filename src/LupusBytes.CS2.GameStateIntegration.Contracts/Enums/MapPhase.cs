using System.Text.Json.Serialization;

namespace LupusBytes.CS2.GameStateIntegration.Contracts.Enums;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum MapPhase
{
    /// <summary>
    /// The game is in freeze time.
    /// </summary>
    Freezetime,

    /// <summary>
    /// The round or game is undergoing.
    /// </summary>
    Live,

    /// <summary>
    /// The game is in warmup.
    /// </summary>
    Warmup,

    /// <summary>
    /// The game is in intermission.
    /// </summary>
    Intermission,
}