using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using QuestBase.Api.Data;
using QuestBase.Api.Features.Projects;

namespace QuestBase.Api.Shared.Auth;

public class ApiKeyAuthenticationHandler
    : AuthenticationHandler<AuthenticationSchemeOptions>
{
    public const string SchemeName = "ApiKey";

    private readonly QuestBaseDbContext _db;

    public ApiKeyAuthenticationHandler(
        IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder,
        QuestBaseDbContext db)
        : base(options, logger, encoder)
    {
        _db = db;
    }

    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        if (!Request.Headers.TryGetValue("X-Api-Key", out var values))
        {
            return AuthenticateResult.NoResult();
        }

        if (values.Count != 1 || string.IsNullOrWhiteSpace(values[0]))
        {
            return AuthenticateResult.Fail("Invalid API key.");
        }

        var keyHash = ApiKeyGenerator.Hash(values[0]!);

        var apiKey = await _db.ApiKeys
            .AsNoTracking()
            .FirstOrDefaultAsync(
                x => x.KeyHash == keyHash && x.RevokedAt == null,
                Context.RequestAborted);

        if (apiKey is null)
        {
            return AuthenticateResult.Fail("Invalid API key.");
        }

        var claims = new[]
        {
            new Claim("project_id", apiKey.ProjectId.ToString()),
            new Claim("api_key_id", apiKey.Id.ToString())
        };

        var identity = new ClaimsIdentity(claims, SchemeName);
        var principal = new ClaimsPrincipal(identity);
        var ticket = new AuthenticationTicket(principal, SchemeName);

        return AuthenticateResult.Success(ticket);
    }
}