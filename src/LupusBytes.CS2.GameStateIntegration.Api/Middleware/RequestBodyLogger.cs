using System.Diagnostics.CodeAnalysis;
using System.Net.Mime;
using System.Text.Json;

namespace LupusBytes.CS2.GameStateIntegration.Api.Middleware;

public partial class RequestBodyLogger(RequestDelegate next, ILogger<RequestBodyLogger> logger)
{
    public async Task Invoke(HttpContext httpContext)
    {
        if (httpContext.Request.Method == HttpMethods.Post && httpContext.Request.ContentLength > 0)
        {
            httpContext.Request.EnableBuffering();
            await LogRequestBody(httpContext.Request, httpContext.RequestAborted);
        }

        await next(httpContext);
    }

    [SuppressMessage("Reliability", "CA2000:Dispose objects before losing scope", Justification = "We don't own httpRequest.Body, so we should not dispose it")]
    private async Task LogRequestBody(HttpRequest httpRequest, CancellationToken cancellationToken)
    {
        var reader = new StreamReader(httpRequest.Body);

        string requestBody;
        if (httpRequest.ContentType?.StartsWith(MediaTypeNames.Application.Json, StringComparison.OrdinalIgnoreCase) == true)
        {
            // Ensure the JSON is minified before logging.
            using var jsonDocument = await JsonDocument.ParseAsync(httpRequest.Body, cancellationToken: cancellationToken);
            requestBody = JsonSerializer.Serialize(jsonDocument.RootElement);
        }
        else
        {
            requestBody = await reader.ReadToEndAsync(cancellationToken);
        }

        LogRequestBody(requestBody);

        // Reset the Stream position, so the next RequestDelegate will read it from the beginning again.
        httpRequest.Body.Position = 0;
    }

    [LoggerMessage(
        EventId = 99,
        Level = LogLevel.Debug,
        Message = "Received payload: {RequestBody}")]
    private partial void LogRequestBody(string requestBody);
}