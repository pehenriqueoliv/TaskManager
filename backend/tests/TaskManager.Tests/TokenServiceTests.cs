using Microsoft.Extensions.Options;
using TaskManager.Api.Common;
using TaskManager.Api.Entities;
using TaskManager.Api.Services;

namespace TaskManager.Tests;

public class TokenServiceTests
{
    private static TokenService CreateService() => new(Options.Create(new JwtOptions
    {
        Issuer = "TaskManager.Api",
        Audience = "TaskManager.Client",
        Key = "chave-de-teste-com-pelo-menos-256-bits-de-entropia!!",
        AccessTokenMinutes = 15,
        RefreshTokenDays = 7
    }));

    [Fact]
    public void CreateAccessToken_ReturnsSignedTokenWithFutureExpiry()
    {
        var service = CreateService();
        var user = new AppUser { Id = Guid.NewGuid(), Email = "alice@test.local" };

        var (token, expiresAt) = service.CreateAccessToken(user);

        Assert.Equal(3, token.Split('.').Length);
        Assert.True(expiresAt > DateTimeOffset.UtcNow);
    }

    [Fact]
    public void CreateRefreshToken_HashMatchesPlainToken()
    {
        var service = CreateService();

        var (token, hash, expiresAt) = service.CreateRefreshToken();

        Assert.Equal(hash, service.HashRefreshToken(token));
        Assert.True(expiresAt > DateTimeOffset.UtcNow);
    }

    [Fact]
    public void HashRefreshToken_IsDeterministic()
    {
        var service = CreateService();

        Assert.Equal(service.HashRefreshToken("same-input"), service.HashRefreshToken("same-input"));
        Assert.NotEqual(service.HashRefreshToken("a"), service.HashRefreshToken("b"));
    }

    [Fact]
    public void CreateRefreshToken_ProducesUniqueTokens()
    {
        var service = CreateService();

        var first = service.CreateRefreshToken();
        var second = service.CreateRefreshToken();

        Assert.NotEqual(first.Token, second.Token);
    }
}
