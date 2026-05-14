namespace LupusBytes.CS2.GameStateIntegration.Api.Endpoints;

internal static class Constants
{
    /// <summary>
    /// Maximum accepted request body size for the ingestion endpoint.
    /// A real CS2 game state payload is only a few KB;
    /// 64KiB leaves generous head-room for users who have allplayers_* enabled while preventing large-payload DoS.
    /// </summary>
    internal const long MaxIngestionRequestBodySizeBytes = 64 * 1024;
}