using LupusBytes.CS2.GameStateIntegration.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace LupusBytes.CS2.GameStateIntegration.Api;

public static class Endpoints
{
    public static void MapIngestionEndpoint(this WebApplication app)
        => app.MapPost("/", (
            [FromServices] GameState gameState,
            [FromBody] Data data) =>
        {
            gameState.ProcessEvent(data);
            return Results.NoContent();
        });
}