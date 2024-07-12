using LupusBytes.CS2.GameStateIntegration.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace LupusBytes.CS2.GameStateIntegration.Api;

public static class Endpoints
{
    public static void MapGetEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet("{steamId}/map", (
            [FromServices] GameStateService gameStateService,
            string steamId) =>
        {
            var map = gameStateService.GetRound(steamId);
            return map is null
                ? Results.NotFound()
                : Results.Ok(map);
        });

        app.MapGet("{steamId}/round", (
            [FromServices] GameStateService gameStateService,
            string steamId) =>
        {
            var round = gameStateService.GetRound(steamId);
            return round is null
                ? Results.NotFound()
                : Results.Ok(round);
        });

        app.MapGet("{steamId}/player", (
            [FromServices] GameStateService gameStateService,
            string steamId) =>
        {
            var player = gameStateService.GetPlayer(steamId);
            return player is null
                ? Results.NotFound()
                : Results.Ok(player);
        });
    }

    public static void MapIngestionEndpoint(this IEndpointRouteBuilder app)
        => app.MapPost("/", (
            [FromServices] GameStateService gameState,
            [FromBody] GameStateData data) =>
        {
            gameState.ProcessEvent(data);
            return Results.NoContent();
        });

    public static void MapIngestionDebugEndpoint(this IEndpointRouteBuilder app)
        => app.MapPost("/debug", async (HttpRequest request) =>
        {
            string body;
            using (var stream = new StreamReader(request.Body))
            {
                body = await stream.ReadToEndAsync();
            }

            Console.WriteLine("*** EVENT RECEIVED ***");
            Console.WriteLine(body);

            return TypedResults.NoContent();
        });
}