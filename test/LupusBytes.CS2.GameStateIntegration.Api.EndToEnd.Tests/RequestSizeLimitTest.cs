using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Text;
using LupusBytes.CS2.GameStateIntegration.Api.Endpoints;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;

namespace LupusBytes.CS2.GameStateIntegration.Api.EndToEnd.Tests;

public class RequestSizeLimitTest
{
    [Theory]
    [InlineData(HttpStatusCode.BadRequest, 1)]
    [InlineData(HttpStatusCode.BadRequest, Endpoints.Constants.MaxIngestionRequestBodySizeBytes)]
    [InlineData(HttpStatusCode.RequestEntityTooLarge, Endpoints.Constants.MaxIngestionRequestBodySizeBytes + 1)]
    [SuppressMessage("ReSharper", "ShortLivedHttpClient", Justification = "Not valid for this test scenario")]
    public async Task MaxIngestionRequestBodySizeBytes_is_respected(
        HttpStatusCode expectedStatusCode,
        long requestBodySizeBytes)
    {
        var builder = WebApplication.CreateBuilder();
        builder.WebHost.UseKestrel(k =>
        {
            k.Listen(IPAddress.Loopback, 0);
        });

        var app = builder.Build();
        app.MapCS2IngestionEndpoint();

        await app.StartAsync(TestContext.Current.CancellationToken);

        try
        {
            // 'x' is U+0078, a basic ASCII character, so it's exactly 1 byte in UTF-8.
            var body = new string('x', (int)requestBodySizeBytes);
            using var client = new HttpClient();
            using var content = new StringContent(body, Encoding.UTF8, "application/json");
            using var response = await client.PostAsync(app.Urls.First(), content, TestContext.Current.CancellationToken);

            response.StatusCode.Should().Be(expectedStatusCode);
        }
        finally
        {
            await app.StopAsync(TestContext.Current.CancellationToken);
            await app.DisposeAsync();
        }
    }
}
