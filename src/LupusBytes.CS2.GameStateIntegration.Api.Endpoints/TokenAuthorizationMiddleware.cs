using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace LupusBytes.CS2.GameStateIntegration.Api.Endpoints;

/// <summary>
/// Validates the <c>auth.token</c> field of incoming Game State Integration payloads
/// and short-circuits the request with <c>401 Unauthorized</c> when it does not match the expected token.
/// </summary>
[SuppressMessage("Performance", "CA1848:Use the LoggerMessage delegates", Justification = "TODO")]
internal class TokenAuthorizationMiddleware(
    RequestDelegate next,
    string expectedToken,
    ILogger<TokenAuthorizationMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        context.Request.EnableBuffering();

        var actualToken = string.Empty;

        using var json = await JsonDocument.ParseAsync(context.Request.Body, cancellationToken: context.RequestAborted);
        if (json.RootElement.TryGetProperty("auth", out var authElement) && authElement.TryGetProperty("token", out var tokenElement))
        {
            actualToken = tokenElement.ToString();
        }

        if (actualToken != expectedToken)
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            logger.LogWarning(
                "Discarding data from {IPAddress} with incorrect auth token '{Token}'",
                context.Connection.RemoteIpAddress,
                actualToken);

            return;
        }

        context.Request.Body.Position = 0;
        await next(context);
    }
}