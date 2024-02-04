using System.Text.Json.Serialization;

namespace LupusBytes.CS2.GameStateIntegration.Contracts;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum Mode
{
    Competitive,
    Casual,
    Deathmatch,
}