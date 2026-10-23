namespace QuestBase.Api.Domain.Projects;

public class ApiKey
{
    public Guid Id { get; set; }
    public Guid ProjectId { get; set; }
    public string Name { get; set; } = default!;
    public string KeyPrefix { get; set; } = default!;
    public string KeyHash { get; set; } = default!;
    public DateTime CreatedAt { get; set; }
    public DateTime? RevokedAt { get; set; }
}