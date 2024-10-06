using System;
using System.Linq;
using ToDoListAPI.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ToDoListAPI.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<ToDoContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddControllers();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<ToDoContext>();
    dbContext.Database.EnsureCreated();

    if (!dbContext.ToDoItems.Any())
    {
        dbContext.ToDoItems.AddRange(new[]
        {
            new ToDoItem { Title = "Buy groceries", Description = "Milk, Bread, Eggs", IsCompleted = false },
            new ToDoItem { Title = "Complete assignment", Description = "Finish the ToDoList API project", IsCompleted = false },
            new ToDoItem { Title = "Call the bank", Description = "Discuss account options", IsCompleted = true },
            new ToDoItem { Title = "Schedule doctor appointment", Description = "Annual check-up", IsCompleted = false }
        });

        dbContext.SaveChanges();
    }
}

app.MapGet("/api/toDoItems", async (ToDoContext context) =>
    await context.ToDoItems.ToListAsync());

app.MapGet("/api/toDoItems/{id}", async (int id, ToDoContext context) =>
    await context.ToDoItems.FindAsync(id) is ToDoItem toDoItem ? Results.Ok(toDoItem) : Results.NotFound());

app.MapPost("/api/toDoItems", async (ToDoItem toDoItem, ToDoContext context) =>
{
    context.ToDoItems.Add(toDoItem);
    await context.SaveChangesAsync();
    return Results.Created($"/api/toDoItems/{toDoItem.Id}", toDoItem);
});

app.MapPut("/api/toDoItems/{id}", async (int id, ToDoItem updatedToDoItem, ToDoContext context) =>
{
    var toDoItem = await context.ToDoItems.FindAsync(id);
    if (toDoItem is null) return Results.NotFound();

    toDoItem.Title = updatedToDoItem.Title;
    toDoItem.Description = updatedToDoItem.Description;
    toDoItem.IsCompleted = updatedToDoItem.IsCompleted;

    await context.SaveChangesAsync();
    return Results.NoContent();
});

app.MapDelete("/api/toDoItems/{id}", async (int id, ToDoContext context) =>
{
    var toDoItem = await context.ToDoItems.FindAsync(id);
    if (toDoItem is null) return Results.NotFound();

    context.ToDoItems.Remove(toDoItem);
    await context.SaveChangesAsync();
    return Results.NoContent();
});

app.MapGet("/weatherforecast", () =>
{
    var summaries = new[]
    {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

    var forecast = Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast(DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
        Random.Shared.Next(-20, 55),
        summaries[Random.Shared.Next(summaries.Length)])
    ).ToArray();

    return forecast;
})
.WithName("GetWeatherForecast")
.WithOpenApi();

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
