using Microsoft.EntityFrameworkCore;
using QuestBase.Api.Data;
using QuestBase.Api.Entities;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<QuestBaseDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("QuestBase")));

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapGet("/health", () => Results.Ok(new
{
    status = "ok",
    service = "QuestBase.Api",
    time = DateTime.UtcNow
}));

app.MapGet("/demo/projects", async (QuestBaseDbContext db) =>
{
    var projects = await db.Projects
        .OrderByDescending(x => x.CreatedAt)
        .Take(10)
        .ToListAsync();

    return Results.Ok(projects);
});

app.MapPost("/demo/projects", async (
    CreateProjectRequest request,
    QuestBaseDbContext db) =>
{
    var project = new Project
    {
        Id = Guid.NewGuid(),
        Name = request.Name,
        Slug = request.Name.ToLower().Replace(" ", "-"),
        CreatedAt = DateTime.UtcNow
    };

    db.Projects.Add(project);
    await db.SaveChangesAsync();

    return Results.Created($"/demo/projects/{project.Id}", project);
});

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
{
    var forecast =  Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
})
.WithName("GetWeatherForecast")
.WithOpenApi();

app.Run();

public record CreateProjectRequest(string Name);
record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
