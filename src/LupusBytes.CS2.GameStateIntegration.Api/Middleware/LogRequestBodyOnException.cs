namespace LupusBytes.CS2.GameStateIntegration.Api.Middleware;

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