using System.Text.Json.Serialization;

namespace LupusBytes.CS2.GameStateIntegration.Contracts;

// Omitted:
// "matches_won_this_series": 0
public record TeamMapDetails(
    int Score,
    [property: JsonPropertyName("consecutive_round_losses")] int ConsecutiveRoundLosses,
    [property: JsonPropertyName("timeouts_remaining")] int TimeoutsRemaining);