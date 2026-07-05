using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using TaskManager.Api.Common;
using TaskManager.Api.Data;
using TaskManager.Api.Dtos;
using TaskManager.Api.Entities;

namespace TaskManager.Api.Services;

public class TaskService(AppDbContext db) : ITaskService
{
    private static readonly Expression<Func<TaskItem, TaskResponse>> ProjectToResponse =
        t => new TaskResponse(t.Id, t.Title, t.Description, t.Status, t.Priority, t.DueDate, t.ProjectId, t.CreatedAt);

    private static readonly Func<TaskItem, TaskResponse> ToResponse = ProjectToResponse.Compile();

    public async Task<IReadOnlyList<TaskResponse>> GetAllAsync(
        Guid ownerId,
        Guid projectId,
        TaskItemStatus? status,
        TaskPriority? priority,
        CancellationToken ct = default)
    {
        await EnsureProjectOwnedAsync(ownerId, projectId, ct);

        var query = db.Tasks.Where(t => t.ProjectId == projectId);

        if (status is not null)
            query = query.Where(t => t.Status == status.Value);

        if (priority is not null)
            query = query.Where(t => t.Priority == priority.Value);

        return await query
            .OrderByDescending(t => t.CreatedAt)
            .Select(ProjectToResponse)
            .ToListAsync(ct);
    }

    public async Task<TaskResponse> CreateAsync(Guid ownerId, Guid projectId, CreateTaskRequest request, CancellationToken ct = default)
    {
        await EnsureProjectOwnedAsync(ownerId, projectId, ct);

        if (request.DueDate is { } dueDate && dueDate < DateTimeOffset.UtcNow)
            throw new BadRequestException("DueDate cannot be in the past.");

        var task = new TaskItem
        {
            Id = Guid.NewGuid(),
            Title = request.Title,
            Description = request.Description,
            Status = request.Status,
            Priority = request.Priority,
            DueDate = request.DueDate,
            ProjectId = projectId,
            CreatedAt = DateTimeOffset.UtcNow
        };

        db.Tasks.Add(task);
        await db.SaveChangesAsync(ct);

        return ToResponse(task);
    }

    public async Task<TaskResponse> UpdateAsync(Guid ownerId, Guid projectId, Guid id, UpdateTaskRequest request, CancellationToken ct = default)
    {
        var task = await db.Tasks
            .FirstOrDefaultAsync(t => t.Id == id && t.ProjectId == projectId && t.Project.OwnerId == ownerId, ct);

        if (task is null)
            throw new NotFoundException($"Task '{id}' was not found.");

        task.Title = request.Title;
        task.Description = request.Description;
        task.Status = request.Status;
        task.Priority = request.Priority;
        task.DueDate = request.DueDate;

        await db.SaveChangesAsync(ct);

        return ToResponse(task);
    }

    public async Task DeleteAsync(Guid ownerId, Guid projectId, Guid id, CancellationToken ct = default)
    {
        var task = await db.Tasks
            .FirstOrDefaultAsync(t => t.Id == id && t.ProjectId == projectId && t.Project.OwnerId == ownerId, ct);

        if (task is null)
            throw new NotFoundException($"Task '{id}' was not found.");

        db.Tasks.Remove(task);
        await db.SaveChangesAsync(ct);
    }

    private async Task EnsureProjectOwnedAsync(Guid ownerId, Guid projectId, CancellationToken ct)
    {
        var exists = await db.Projects.AnyAsync(p => p.Id == projectId && p.OwnerId == ownerId, ct);

        if (!exists)
            throw new NotFoundException($"Project '{projectId}' was not found.");
    }
}
