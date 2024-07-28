using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace LupusBytes.CS2.GameStateIntegration.Contracts;

/// <summary>
/// Raw data from CS2.
/// Contains game state seperated into independent blocks.
/// </summary>
/// <param name="Map">
/// Information about the current map.<br/>
/// Enabled by including <code>"map": "1"</code> in the game state cfg file.
/// </param>
/// <param name="Round">
/// Information about the current round.<br/>
/// Enabled by including <code>"round": "1"</code> in the game state cfg file.
/// </param>
/// <param name="Player">
/// Information about the current or observed player.<br/>
/// Enabled by including <code>"player_id": "1"</code> and <code>"player_state": "1"</code> in the game state cfg file.
/// </param>
/// <param name="Provider">
/// Required information about the game and steam account that is providing data.<br />
/// Enabled by including <code>"provider": "1"</code> in the game state cfg file.
/// </param>
public record GameStateData(
    [property: JsonPropertyName("provider"), Required] Provider? Provider,
    [property: JsonPropertyName("map")] Map? Map,
    [property: JsonPropertyName("round")] Round? Round,
    [property: JsonPropertyName("player")] PlayerWithState? Player);