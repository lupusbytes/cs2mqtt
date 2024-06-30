using System.Text.Json.Serialization;

namespace LupusBytes.CS2.GameStateIntegration.Contracts;

public record PlayerState(
    [property: JsonPropertyName("health")] int Health,
    [property: JsonPropertyName("armor")] int Armor,
    [property: JsonPropertyName("helmet")] bool Helmet,
    [property: JsonPropertyName("flashed")] byte Flashed,
    [property: JsonPropertyName("smoked")] byte Smoked,
    [property: JsonPropertyName("burning")] byte Burning,
    [property: JsonPropertyName("money")] int Money,
    [property: JsonPropertyName("round_kills")] int RoundKills,
    [property: JsonPropertyName("round_killhs")] int RoundKillHeadshots,
    [property: JsonPropertyName("equip_value")] int EquipmentValue);