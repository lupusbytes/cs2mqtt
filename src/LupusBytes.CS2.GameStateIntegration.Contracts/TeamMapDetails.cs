using System.Text.Json.Serialization;

namespace LupusBytes.CS2.GameStateIntegration.Contracts;

public record TeamMapDetails(
    [property: JsonPropertyName("score")] int Score,
    [property: JsonPropertyName("consecutive_round_losses")] int ConsecutiveRoundLosses,
    [property: JsonPropertyName("timeouts_remaining")] int TimeoutsRemaining,
    [property: JsonPropertyName("matches_won_this_series")] int MatchesWonThisSeries);