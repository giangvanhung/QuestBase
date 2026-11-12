using System.Security.Claims;
using QuestBase.Api.Domain.Projects;
using QuestBase.Api.Shared.Extensions;
using Microsoft.AspNetCore.Http.HttpResults;

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
            var name = request.Name?.Trim();
            if (string.IsNullOrWhiteSpace(name) || name.Length >= 100)
            {
                return TypedResults.ValidationProblem(new Dictionary<string, string[]>
                {
                    [nameof(request.Name)] = ["Name is required and must be shorter than 100 characters."]
                });
            }
            
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

        group.MapGet("/{projectId:guid}", async Task<Results<
        NotFound,
        Ok<Project>>> (
        ProjectService service,
        Guid projectId,
        ClaimsPrincipal user,
        CancellationToken ct) => {
            var ownerUserId = user.GetUserId();
            var project = await service.GetByIdAsync(ownerUserId, projectId, ct);
            return project is null
                ? TypedResults.NotFound()
                : TypedResults.Ok(project);
        });
    }
}