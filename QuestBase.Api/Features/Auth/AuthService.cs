using Microsoft.EntityFrameworkCore;
using QuestBase.Api.Data;
using QuestBase.Api.Domain.Users;
using QuestBase.Api.Shared.Exceptions;

namespace QuestBase.Api.Features.Auth;

public class AuthService
{
    private readonly QuestBaseDbContext _db;
    private readonly JwtTokenService _jwt;
    public AuthService(QuestBaseDbContext db, JwtTokenService jwt)
    {
        _db = db;
        _jwt = jwt;
    }

    public async Task RegisterAsync(
        RegisterRequest request,
        CancellationToken ct)
    {
        var email = request.Email.Trim().ToLowerInvariant();

        var exists = await _db.Users
            .AnyAsync(x => x.Email == email, ct);

        if (exists)
        {
            throw new ConflictException("Email already registered.");
        }

        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = email,
            DisplayName = request.DisplayName,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
            CreatedAt = DateTime.UtcNow
        };

        _db.Users.Add(user);
        await _db.SaveChangesAsync(ct);
    }

    public async Task<LoginResult> LoginAsync(
        LoginRequest request,
        CancellationToken ct)
    {
        var email = request.Email.Trim().ToLowerInvariant();

        var user = await _db.Users
            .FirstOrDefaultAsync(x => x.Email == email, ct);

        if (user is null ||
            !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
        {
            throw new UnauthorizedException("Invalid email or password.");
        }

        var token = _jwt.CreateToken(user.Id, user.Email);

        return new LoginResult(
            user.Id,
            user.Email,
            user.DisplayName,
            token);
    }
}