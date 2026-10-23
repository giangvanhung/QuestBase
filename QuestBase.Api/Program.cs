using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using QuestBase.Api.Data;
using QuestBase.Api.Domain.Projects;
using QuestBase.Api.Features.Auth;
using QuestBase.Api.Modules.Auth;
using QuestBase.Api.Shared.Extensions;

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
        options.MapInboundClaims = false;

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
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Description = "Enter your JWT token only. Do not type the word Bearer."
    });

    options.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

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

app.MapPost("/v1/projects", async (
    CreateProjectRequest request,
    QuestBaseDbContext db,
    ClaimsPrincipal user,
    CancellationToken ct) =>
{
    var ownerUserId = user.GetUserId();

    var project = new Project
    {
        Id = Guid.NewGuid(),
        OwnerUserId = ownerUserId,
        Name = request.Name,
        Slug = request.Name.ToLowerInvariant().Replace(" ", "-"),
        CreatedAt = DateTime.UtcNow
    };

    db.Projects.Add(project);
    await db.SaveChangesAsync(ct);

    return Results.Created($"/v1/projects/{project.Id}", project);
})
.RequireAuthorization();

// Auth endpoints
app.MapAuthEndpoints();
app.Run();
public record CreateProjectRequest(string Name);