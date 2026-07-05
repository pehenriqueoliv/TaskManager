using Microsoft.AspNetCore.Identity;
using Moq;
using TaskManager.Api.Common;
using TaskManager.Api.Dtos;
using TaskManager.Api.Entities;
using TaskManager.Api.Services;

namespace TaskManager.Tests;

public class AuthServiceTests
{
    [Fact]
    public async Task RegisterAsync_ReturnsTokens_AndPersistsRefreshToken()
    {
        using var db = new SqliteInMemoryDatabase(enforceForeignKeys: false);
        var userManager = MockUserManager();
        userManager.Setup(m => m.CreateAsync(It.IsAny<AppUser>(), "Passw0rd!"))
            .ReturnsAsync(IdentityResult.Success);

        var tokenService = new Mock<ITokenService>();
        StubTokens(tokenService, "access-token", "refresh-plain", "refresh-hash");

        AuthResponse response;
        await using (var context = db.CreateContext())
            response = await new AuthService(userManager.Object, context, tokenService.Object)
                .RegisterAsync(new RegisterRequest("alice@test.local", "Passw0rd!"));

        Assert.Equal("access-token", response.AccessToken);
        Assert.Equal("refresh-plain", response.RefreshToken);

        await using var check = db.CreateContext();
        var stored = Assert.Single(check.RefreshTokens);
        Assert.Equal("refresh-hash", stored.TokenHash);
        Assert.Null(stored.RevokedAt);
    }

    [Fact]
    public async Task RegisterAsync_Throws_WhenIdentityFails()
    {
        using var db = new SqliteInMemoryDatabase(enforceForeignKeys: false);
        var userManager = MockUserManager();
        userManager.Setup(m => m.CreateAsync(It.IsAny<AppUser>(), It.IsAny<string>()))
            .ReturnsAsync(IdentityResult.Failed(new IdentityError { Description = "Email já em uso." }));

        await using var context = db.CreateContext();
        var service = new AuthService(userManager.Object, context, Mock.Of<ITokenService>());

        await Assert.ThrowsAsync<BadRequestException>(
            () => service.RegisterAsync(new RegisterRequest("alice@test.local", "Passw0rd!")));
    }

    [Fact]
    public async Task LoginAsync_Throws_WhenUserNotFound()
    {
        using var db = new SqliteInMemoryDatabase(enforceForeignKeys: false);
        var userManager = MockUserManager();
        userManager.Setup(m => m.FindByEmailAsync(It.IsAny<string>())).ReturnsAsync((AppUser?)null);

        await using var context = db.CreateContext();
        var service = new AuthService(userManager.Object, context, Mock.Of<ITokenService>());

        await Assert.ThrowsAsync<UnauthorizedException>(
            () => service.LoginAsync(new LoginRequest("ghost@test.local", "Passw0rd!")));
    }

    [Fact]
    public async Task LoginAsync_Throws_WhenPasswordIsInvalid()
    {
        using var db = new SqliteInMemoryDatabase(enforceForeignKeys: false);
        var user = new AppUser { Id = Guid.NewGuid(), Email = "alice@test.local" };
        var userManager = MockUserManager();
        userManager.Setup(m => m.FindByEmailAsync("alice@test.local")).ReturnsAsync(user);
        userManager.Setup(m => m.CheckPasswordAsync(user, "wrong")).ReturnsAsync(false);

        await using var context = db.CreateContext();
        var service = new AuthService(userManager.Object, context, Mock.Of<ITokenService>());

        await Assert.ThrowsAsync<UnauthorizedException>(
            () => service.LoginAsync(new LoginRequest("alice@test.local", "wrong")));
    }

    [Fact]
    public async Task RefreshAsync_RevokesOldToken_AndIssuesNewPair()
    {
        using var db = new SqliteInMemoryDatabase(enforceForeignKeys: false);
        var userId = Guid.NewGuid();
        await using (var seed = db.CreateContext())
        {
            seed.RefreshTokens.Add(new RefreshToken
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                TokenHash = "old-hash",
                ExpiresAt = DateTimeOffset.UtcNow.AddDays(7),
                CreatedAt = DateTimeOffset.UtcNow
            });
            await seed.SaveChangesAsync();
        }

        var userManager = MockUserManager();
        userManager.Setup(m => m.FindByIdAsync(userId.ToString()))
            .ReturnsAsync(new AppUser { Id = userId, Email = "alice@test.local" });

        var tokenService = new Mock<ITokenService>();
        tokenService.Setup(t => t.HashRefreshToken("old-plain")).Returns("old-hash");
        StubTokens(tokenService, "access-2", "refresh-2-plain", "new-hash");

        AuthResponse response;
        await using (var context = db.CreateContext())
            response = await new AuthService(userManager.Object, context, tokenService.Object)
                .RefreshAsync(new RefreshRequest("old-plain"));

        Assert.Equal("access-2", response.AccessToken);
        Assert.Equal("refresh-2-plain", response.RefreshToken);

        await using var check = db.CreateContext();
        var oldToken = check.RefreshTokens.Single(t => t.TokenHash == "old-hash");
        Assert.NotNull(oldToken.RevokedAt);
        Assert.Contains(check.RefreshTokens, t => t.TokenHash == "new-hash" && t.RevokedAt == null);
    }

    [Fact]
    public async Task RefreshAsync_Throws_WhenTokenIsUnknown()
    {
        using var db = new SqliteInMemoryDatabase(enforceForeignKeys: false);
        var tokenService = new Mock<ITokenService>();
        tokenService.Setup(t => t.HashRefreshToken(It.IsAny<string>())).Returns("no-match");

        await using var context = db.CreateContext();
        var service = new AuthService(MockUserManager().Object, context, tokenService.Object);

        await Assert.ThrowsAsync<UnauthorizedException>(
            () => service.RefreshAsync(new RefreshRequest("anything")));
    }

    [Fact]
    public async Task RefreshAsync_Throws_WhenTokenIsAlreadyRevoked()
    {
        using var db = new SqliteInMemoryDatabase(enforceForeignKeys: false);
        await using (var seed = db.CreateContext())
        {
            seed.RefreshTokens.Add(new RefreshToken
            {
                Id = Guid.NewGuid(),
                UserId = Guid.NewGuid(),
                TokenHash = "revoked-hash",
                ExpiresAt = DateTimeOffset.UtcNow.AddDays(7),
                CreatedAt = DateTimeOffset.UtcNow,
                RevokedAt = DateTimeOffset.UtcNow
            });
            await seed.SaveChangesAsync();
        }

        var tokenService = new Mock<ITokenService>();
        tokenService.Setup(t => t.HashRefreshToken("revoked-plain")).Returns("revoked-hash");

        await using var context = db.CreateContext();
        var service = new AuthService(MockUserManager().Object, context, tokenService.Object);

        await Assert.ThrowsAsync<UnauthorizedException>(
            () => service.RefreshAsync(new RefreshRequest("revoked-plain")));
    }

    private static void StubTokens(Mock<ITokenService> tokenService, string access, string refreshPlain, string refreshHash)
    {
        tokenService.Setup(t => t.CreateAccessToken(It.IsAny<AppUser>()))
            .Returns((access, DateTimeOffset.UtcNow.AddMinutes(15)));
        tokenService.Setup(t => t.CreateRefreshToken())
            .Returns((refreshPlain, refreshHash, DateTimeOffset.UtcNow.AddDays(7)));
    }

    private static Mock<UserManager<AppUser>> MockUserManager()
    {
        var store = new Mock<IUserStore<AppUser>>();
        return new Mock<UserManager<AppUser>>(
            store.Object, null!, null!, null!, null!, null!, null!, null!, null!);
    }
}
