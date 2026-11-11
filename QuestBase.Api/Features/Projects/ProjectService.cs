

using Microsoft.EntityFrameworkCore;
using QuestBase.Api.Data;
using QuestBase.Api.Domain.Projects;

namespace QuestBase.Api.Features.Projects;
class ProjectService
{
    private readonly QuestBaseDbContext _db;
    public ProjectService(QuestBaseDbContext questBaseDbContext)
    {
        _db = questBaseDbContext; 
    }

    public async Task<Project> CreateAsync(
    Guid ownerUserId,
    CreateProjectRequest request,
    CancellationToken ct)
    {
        var project = new Project
        {
            Id = Guid.NewGuid(),
            OwnerUserId = ownerUserId,
            Name = request.Name,
            Slug = request.Name.ToLowerInvariant().Replace(" ", "-"),
            CreatedAt = DateTime.UtcNow
        };

        _db.Projects.Add(project);
        await _db.SaveChangesAsync(ct);
        return project;
    }

    public async Task<List<Project>> GetProjectsAsync(
    Guid ownerUserId,
    CancellationToken ct)
    {

        var projects = await _db.Projects.
        Where(pj => pj.OwnerUserId == ownerUserId)
        .ToListAsync(ct);
        
        return projects;
    }
}