using System.ComponentModel.DataAnnotations;

namespace TaskManager.Api.Dtos;

public record RegisterRequest(
    [Required][EmailAddress] string Email,
    [Required][StringLength(100, MinimumLength = 8)] string Password);

public record LoginRequest(
    [Required][EmailAddress] string Email,
    [Required] string Password);

public record RefreshRequest(
    [Required] string RefreshToken);

public record AuthResponse(
    string AccessToken,
    DateTimeOffset AccessTokenExpiresAt,
    string RefreshToken,
    string TokenType = "Bearer");
