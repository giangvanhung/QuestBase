using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using QuestBase.Api.Data;
using QuestBase.Api.Domain.Projects;
using QuestBase.Api.Features.Auth;
using QuestBase.Api.Modules.Auth;

var builder = WebApplication.CreateBuilder(args);

// Database
builder.Services.AddDbContext<QuestBaseDbContext>(options =>
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("QuestBase")));

// JWT settings
var jwtSettings = builder.Configuration
    .GetSection("Jwt")
    .Get<JwtSettings>()!;

builder.Services.AddSingleton(jwtSettings);
builder.Services.AddScoped<JwtTokenService>();
builder.Services.AddScoped<AuthService>();

// Authentication
builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSettings.Issuer,
            ValidAudience = jwtSettings.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtSettings.SecretKey))
        };
    });

builder.Services.AddAuthorization();

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Public endpoint
app.MapGet("/health", () => Results.Ok(new
{
    status = "ok",
    service = "QuestBase.Api",
    time = DateTime.UtcNow
}));

// Demo project endpoints
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
        Slug = request.Name.ToLowerInvariant().Replace(" ", "-"),
        CreatedAt = DateTime.UtcNow
    };

    db.Projects.Add(project);
    await db.SaveChangesAsync();

    return Results.Created($"/demo/projects/{project.Id}", project);
});

// Auth endpoints
app.MapAuthEndpoints();
app.Run();
public record CreateProjectRequest(string Name);