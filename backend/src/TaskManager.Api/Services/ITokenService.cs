using TaskManager.Api.Entities;

namespace TaskManager.Api.Services;

public interface ITokenService
{
    (string Token, DateTimeOffset ExpiresAt) CreateAccessToken(AppUser user);
    (string Token, string TokenHash, DateTimeOffset ExpiresAt) CreateRefreshToken();
    string HashRefreshToken(string token);
}
