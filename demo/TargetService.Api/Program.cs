var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
{
    var forecast = Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
});

// Add target service endpoints for testing dynamic routing
app.MapGet("/users/list", () => new { Users = new[] { "Alice", "Bob", "Charlie" }, Timestamp = DateTime.UtcNow });

app.MapGet("/users/{id:int}", (int id) => new { Id = id, Name = $"User {id}", Email = $"user{id}@example.com" });

app.MapPost("/users", (object user) => new { Message = "User created", User = user, CreatedAt = DateTime.UtcNow });

app.MapGet("/products/list", () => new { Products = new[] { "Laptop", "Mouse", "Keyboard" }, Timestamp = DateTime.UtcNow });

app.MapGet("/orders/{id:int}/details", (int id) => new { OrderId = id, Status = "Shipped", Items = new[] { "Item1", "Item2" } });

app.MapGet("/health", () => new { Status = "Healthy", Service = "TargetService.Api", Timestamp = DateTime.UtcNow });

// Keep the original test endpoints too
app.MapGet("/api/target/hello", () => new { Message = "Hello from Target Service!", Timestamp = DateTime.UtcNow });

app.MapGet("/api/target/data/{id:int}", (int id) => new { Id = id, Name = $"Item {id}", Value = Random.Shared.Next(1, 100) });

app.MapPost("/api/target/data", (object data) => new { Message = "Data received", ReceivedAt = DateTime.UtcNow, Data = data });

app.Run();

internal record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
