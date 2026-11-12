

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
        var name = request.Name.Trim();
        if (string.IsNullOrWhiteSpace(name) || name.Length >= 100)
        {
            throw new ArgumentException("Project name is required and must be shorter than 100 characters.");
        }
        var project = new Project
        {
            Id = Guid.NewGuid(),
            OwnerUserId = ownerUserId,
            Name = name,
            Slug = GenerateSlug(name),
            CreatedAt = DateTime.UtcNow
        };

        _db.Projects.Add(project);
        await _db.SaveChangesAsync(ct);

        return project;
    }

    private static string GenerateSlug(string name)
    {
        return name.ToLowerInvariant().Replace(' ', '-');
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

    public Task<Project?> GetByIdAsync(
    Guid ownerUserId,
    Guid projectId,
    CancellationToken ct)
    {
        return _db.Projects.FirstOrDefaultAsync(
            pj => pj.Id == projectId && pj.OwnerUserId == ownerUserId,
            ct);
    }
}