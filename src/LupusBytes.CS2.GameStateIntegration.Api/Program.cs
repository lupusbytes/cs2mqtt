using System.Text.Json;
using System.Text.Json.Serialization;
using LupusBytes.CS2.GameStateIntegration.Contracts;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);
builder.Services.Configure<JsonOptions>(options =>
{
    options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.CamelCase));
});
var app = builder.Build();


 
app.MapGet("/", () => "Hello World!");

app.MapPost("/debug", async (HttpRequest request) =>
{
    string body;
    using (var stream = new StreamReader(request.Body))
    {
        body = await stream.ReadToEndAsync();
    }
    Console.WriteLine("START");
    Console.WriteLine(body);
    Console.WriteLine("STOP");

    return TypedResults.NoContent();
});


app.MapPost("/", ([FromBody] GameState data) =>
{
    Console.WriteLine("Got data!");
    
    if (data.Player is not null)
    {
        Console.WriteLine(data.Player);
    }

    if (data.Provider is not null)
    {
        Console.WriteLine("Found provider");
    }
    
    if (data.Round is not null)
    {
        Console.WriteLine($"Phase: {data.Round.Phase}");
        if (data.Round.Bomb.HasValue)
        {
            Console.WriteLine($"Bomb state: {data.Round.Bomb}");
        }
        
        if (data.Round.WinTeam.HasValue)
        {
            Console.WriteLine($"{data.Round.WinTeam} win the round");
        }
    }
});
app.Run();