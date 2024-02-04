using System.Text.Json.Serialization;

namespace LupusBytes.CS2.GameStateIntegration.Contracts;

public record PlayerState(
    int Health,
    int Armor,
    bool Helmet,
    int Flashed,
    int Smoked,
    int Burning,
    int Money,
    [property: JsonPropertyName("round_kills")] int RoundKills,
    [property: JsonPropertyName("round_killhs")] int RoundKillHeadshots,
    [property: JsonPropertyName("equip_value")] int EquipmentValue);