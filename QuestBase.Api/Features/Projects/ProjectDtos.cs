namespace QuestBase.Api.Features.Projects;
public sealed record CreateProjectRequest(string Name);
public record CreateApiKeyRequest(string Name);
public record CreateApiKeyResponse(
    Guid Id,
    string Name,
    string Key,
    string KeyPrefix,
    DateTime CreatedAt);
public record ApiKeyResponse(
    Guid Id,
    string Name,
    string KeyPrefix,
    DateTime CreatedAt,
    DateTime? RevokedAt);