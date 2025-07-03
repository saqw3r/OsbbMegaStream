using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.Extensions.Hosting;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<JsonOptions>(options =>
{
    options.SerializerOptions.PropertyNamingPolicy = null; // Use property names as-is
});

var app = builder.Build();

app.MapPost("/webhook", async (HttpContext context) =>
{
    var payload = await context.Request.ReadFromJsonAsync<WebhookPayload>();
    if (payload is null)
        return Results.BadRequest();

    // Process the payload as needed
    Console.WriteLine($"Path: {payload.path}, SourceType: {payload.source_type}, SourceId: {payload.source_id}");

    return Results.Ok();
});

app.Run();

public class WebhookPayload
{
    public string path { get; set; }
    public string source_type { get; set; }
    public string source_id { get; set; }
}
