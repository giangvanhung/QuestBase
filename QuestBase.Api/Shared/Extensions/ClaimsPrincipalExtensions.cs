// Shared/Extensions/ClaimsPrincipalExtensions.cs
using System.Security.Claims;

namespace QuestBase.Api.Shared.Extensions;

public static class ClaimsPrincipalExtensions
{
    public static Guid GetUserId(this ClaimsPrincipal user)
    {
        var value = user.FindFirstValue("sub")
            ?? user.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrWhiteSpace(value) ||
            !Guid.TryParse(value, out var userId))
        {
            throw new UnauthorizedAccessException("Invalid user token.");
        }

        return userId;
    }
}