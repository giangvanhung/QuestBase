using System.Security.Claims;
using QuestBase.Api.Domain.Projects;
using QuestBase.Api.Shared.Extensions;

namespace QuestBase.Api.Features.Projects;

public static class ProjectEndpoints
{
    public static void MapProjectEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/v1/projects")
            .RequireAuthorization();

        group.MapPost("/", async (
            CreateProjectRequest request,
            ProjectService service,
            ClaimsPrincipal user,
            CancellationToken ct) =>
        {
            var ownerUserId = user.GetUserId();
            var project = await service.CreateAsync(ownerUserId, request, ct);
            return Results.Created($"/v1/projects/{project.Id}", project);
        });

        group.MapGet("/", async(
        ProjectService service,
        ClaimsPrincipal user,
        CancellationToken ct) => {
            var ownerUserId = user.GetUserId();
            var projects = await service.GetProjectsAsync(ownerUserId, ct);

            return TypedResults.Ok(projects);
        });
    }
}