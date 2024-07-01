using LupusBytes.CS2.GameStateIntegration.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace LupusBytes.CS2.GameStateIntegration.Api;

public static class Endpoints
{
    public static void MapGetEndpoints(this WebApplication app)
    {
        app.MapGet("/map", ([FromServices] GameState gameState)
            => gameState.Map is null
                ? Results.NotFound()
                : Results.Ok(gameState.Map));

        app.MapGet("/round", ([FromServices] GameState gameState)
            => gameState.Round is null
                ? Results.NotFound()
                : Results.Ok(gameState.Round));

        app.MapGet("/player", ([FromServices] GameState gameState)
            => gameState.Player is null
                ? Results.NotFound()
                : Results.Ok(gameState.Player));

        app.MapGet("/provider", ([FromServices] GameState gameState)
            => gameState.Provider is null
                ? Results.NotFound()
                : Results.Ok(gameState.Provider));
    }

    public static void MapIngestionEndpoint(this WebApplication app)
        => app.MapPost("/", (
            [FromServices] GameState gameState,
            [FromBody] Data data) =>
        {
            gameState.ProcessEvent(data);
            return Results.NoContent();
        });
}