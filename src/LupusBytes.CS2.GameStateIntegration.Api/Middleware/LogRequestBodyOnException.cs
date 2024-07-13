using System.Diagnostics.CodeAnalysis;

namespace LupusBytes.CS2.GameStateIntegration.Api.Middleware;

[SuppressMessage("Naming", "CA1711:Identifiers should not have incorrect suffix", Justification = "It's fine.")]
[SuppressMessage("Major Code Smell", "S2166:Classes named like \"Exception\" should extend \"Exception\" or a subclass", Justification = "It's fine")]
public class LogRequestBodyOnException(RequestDelegate next)
{
    public async Task Invoke(HttpContext httpContext)
    {
        if (httpContext.Request.Method == HttpMethods.Post && httpContext.Request.ContentLength > 0)
        {
            httpContext.Request.EnableBuffering();
        }

        try
        {
            await next(httpContext);
        }
        catch (Exception)
        {
            await LogRequestBody(httpContext);
            throw;
        }
    }

    private static async Task LogRequestBody(HttpContext httpContext)
    {
        httpContext.Request.Body.Position = 0;
        using var reader = new StreamReader(httpContext.Request.Body);
        var requestBody = await reader.ReadToEndAsync(httpContext.RequestAborted);

        Console.WriteLine($"Exception on data:{Environment.NewLine}{requestBody}");
    }
}