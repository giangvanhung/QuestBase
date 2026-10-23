using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace QuestBase.Api.Features.Auth;

public static class AuthEndpoints
{
    public static void MapAuthEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/v1/auth");

        group.MapPost("/register", async (
            RegisterRequest request,
            AuthService service,
            CancellationToken ct) =>
        {
            await service.RegisterAsync(request, ct);
            return Results.Created("/v1/auth/me", new
            {
                message = "Registered successfully"
            });
        });

        group.MapPost("/login", async (
            LoginRequest request,
            AuthService service,
            CancellationToken ct) =>
        {
            var result = await service.LoginAsync(request, ct);
            return Results.Ok(result);
        });

        group.MapGet("/me", (
            System.Security.Claims.ClaimsPrincipal user) =>
        {
            return Results.Ok(new
            {
                userId = user.FindFirst("sub")?.Value,
                email = user.FindFirst("email")?.Value
            });
        }).RequireAuthorization();
    }
}