using System.ComponentModel.DataAnnotations;
using TaskManager.Api.Entities;

namespace TaskManager.Api.Dtos;

public record CreateTaskRequest(
    [Required][StringLength(200, MinimumLength = 1)] string Title,
    [StringLength(2000)] string? Description,
    TaskItemStatus Status,
    TaskPriority Priority,
    DateTimeOffset? DueDate);

public record UpdateTaskRequest(
    [Required][StringLength(200, MinimumLength = 1)] string Title,
    [StringLength(2000)] string? Description,
    TaskItemStatus Status,
    TaskPriority Priority,
    DateTimeOffset? DueDate);

public record TaskResponse(
    Guid Id,
    string Title,
    string? Description,
    TaskItemStatus Status,
    TaskPriority Priority,
    DateTimeOffset? DueDate,
    Guid ProjectId,
    DateTimeOffset CreatedAt);
