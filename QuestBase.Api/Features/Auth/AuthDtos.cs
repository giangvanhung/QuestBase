namespace QuestBase.Api.Features.Auth;

public record RegisterRequest(
    string Email,
    string Password,
    string DisplayName);

public record LoginRequest(
    string Email,
    string Password);

public record LoginResult(
    Guid UserId,
    string Email,
    string DisplayName,
    string AccessToken);