using LupusBytes.CS2.GameStateIntegration.Contracts;

namespace LupusBytes.CS2.GameStateIntegration;

public class GameStateOptions
{
    public const string Section = nameof(GameStateOptions);

    /// <summary>
    /// If set to <see langword="true" />, data from players with a different <see cref="SteamId64" /> than the <see cref="Provider.SteamId64"/> will be ignored.
    /// </summary>
    public bool IgnoreSpectatedPlayers { get; set; } = true;

    /// <summary>
    /// Counter-Strike 2 client authentication token.
    /// </summary>
    public string Token { get; set; } = null!;

    /// <summary>
    /// The duration of inactivity after which a SteamID is considered disconnected, prompting the cleanup of their resources.
    /// This value should be a tiny bit longer than the heartbeat defined in the gamestate_integration.cfg
    /// </summary>
    public double TimeoutInSeconds { get; set; } = 60.5;

    /// <summary>
    /// The amount of time to wait between each execution of the background task that attempts to find disconnected SteamIDs.
    /// </summary>
    public double TimeoutCleanupIntervalInSeconds { get; set; } = 15;
}