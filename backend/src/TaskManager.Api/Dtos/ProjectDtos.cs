using System.ComponentModel.DataAnnotations;

namespace TaskManager.Api.Dtos;

public record CreateProjectRequest(
    [Required][StringLength(120, MinimumLength = 1)] string Name,
    [StringLength(2000)] string? Description);

public record ProjectResponse(
    Guid Id,
    string Name,
    string? Description,
    DateTimeOffset CreatedAt);
