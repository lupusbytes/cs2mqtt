using System.Diagnostics.CodeAnalysis;
using LupusBytes.CS2.GameStateIntegration.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace LupusBytes.CS2.GameStateIntegration.Api.Endpoints;

[SuppressMessage("ReSharper", "InconsistentNaming", Justification = "CS is a specific two-letter acronym for Counter-Strike. Two-letter acronyms should be fully upper or lowercase")]
public static class EndpointRouteBuilderExtensions
{
    public static void MapCS2GetEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet("{steamId}/map", (
            [FromServices] IGameStateService gameStateService,
            string steamId) =>
        {
            var map = gameStateService.GetMap(steamId);
            return map is null
                ? Results.NotFound()
                : Results.Ok(map);
        });

        app.MapGet("{steamId}/round", (
            [FromServices] IGameStateService gameStateService,
            string steamId) =>
        {
            var round = gameStateService.GetRound(steamId);
            return round is null
                ? Results.NotFound()
                : Results.Ok(round);
        });

        app.MapGet("{steamId}/player", (
            [FromServices] IGameStateService gameStateService,
            string steamId) =>
        {
            var player = gameStateService.GetPlayer(steamId);
            return player is null
                ? Results.NotFound()
                : Results.Ok(player);
        });
    }

    public static void MapCS2IngestionEndpoint(this IEndpointRouteBuilder app)
        => app.MapPost("/", (
            [FromServices] IGameStateService gameStateService,
            [FromBody] GameStateData data) =>
        {
            gameStateService.ProcessEvent(data);
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