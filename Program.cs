var builder = WebApplication.CreateBuilder(args);

// Setting up necessary services for the app
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();  // Adding Swagger for API documentation

// Building the app
var app = builder.Build();

// Setting up the app's HTTP request pipeline, enabling Swagger for dev environment
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Enforcing HTTPS for secure communication
app.UseHttpsRedirection();

// Weather forecast summaries (randomly selecting from this list)
var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

// Defining the /weatherforecast API endpoint
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
    return forecast;  // Returning the weather forecast data
})
.WithName("GetWeatherForecast")  // Giving a name to the route
.WithOpenApi();  // Ensuring the API is documented by OpenAPI

app.Run();  // Running the app

// Record type to represent a weather forecast
record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);  // Formula to convert Celsius to Fahrenheit
}

