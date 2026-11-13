

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

    public async Task<CreateApiKeyResponse?> CreateApiKeyAsync(
    Guid ownerUserId,
    Guid projectId,
    CreateApiKeyRequest request,
    CancellationToken ct)
    {
        var project = await GetByIdAsync(ownerUserId, projectId, ct);

        if (project is null)
        {
            return null;
        }

        var name = request.Name?.Trim();

        if (string.IsNullOrWhiteSpace(name) || name.Length > 100)
        {
            throw new ArgumentException(
                "API key name is required and must not exceed 100 characters.");
        }

        var rawKey = ApiKeyGenerator.Generate();

        var apiKey = new ApiKey
        {
            Id = Guid.NewGuid(),
            ProjectId = project.Id,
            Name = name,
            KeyPrefix = rawKey[..11],
            KeyHash = ApiKeyGenerator.Hash(rawKey),
            CreatedAt = DateTime.UtcNow
        };

        _db.Set<ApiKey>().Add(apiKey);
        await _db.SaveChangesAsync(ct);

        return new CreateApiKeyResponse(
            apiKey.Id,
            apiKey.Name,
            rawKey,
            apiKey.KeyPrefix,
            apiKey.CreatedAt);
    }

    public async Task<bool> RevokeApiKeyAsync(
        Guid ownerUserId,
        Guid projectId,
        Guid apiKeyId,
        CancellationToken ct)
    {
        var project = await GetByIdAsync(ownerUserId, projectId, ct);

        if (project is null)
        {
            return false;
        }

        var apiKey = await _db.ApiKeys.FirstOrDefaultAsync(
            x => x.Id == apiKeyId && x.ProjectId == projectId,
            ct);

        if (apiKey is null)
        {
            return false;
        }

        if (apiKey.RevokedAt is null)
        {
            apiKey.RevokedAt = DateTime.UtcNow;
            await _db.SaveChangesAsync(ct);
        }

        return true;
    }

    public async Task<List<ApiKeyResponse>?> GetApiKeysAsync(
        Guid ownerUserId,
        Guid projectId,
        CancellationToken ct)
    {
        var project = await GetByIdAsync(ownerUserId, projectId, ct);

        if (project is null)
        {
            return null;
        }

        return await _db.ApiKeys
            .AsNoTracking()
            .Where(x => x.ProjectId == projectId)
            .OrderByDescending(x => x.CreatedAt)
            .Select(x => new ApiKeyResponse(
                x.Id,
                x.Name,
                x.KeyPrefix,
                x.CreatedAt,
                x.RevokedAt))
            .ToListAsync(ct);
    }
}