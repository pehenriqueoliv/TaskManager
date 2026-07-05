using Microsoft.EntityFrameworkCore;
using TaskManager.Api.Common;
using TaskManager.Api.Data;
using TaskManager.Api.Dtos;
using TaskManager.Api.Entities;

namespace TaskManager.Api.Services;

public class ProjectService(AppDbContext db) : IProjectService
{
    public async Task<IReadOnlyList<ProjectResponse>> GetAllAsync(Guid ownerId, CancellationToken ct = default)
    {
        return await db.Projects
            .Where(p => p.OwnerId == ownerId)
            .OrderByDescending(p => p.CreatedAt)
            .Select(p => new ProjectResponse(p.Id, p.Name, p.Description, p.CreatedAt))
            .ToListAsync(ct);
    }

    public async Task<ProjectResponse> GetByIdAsync(Guid ownerId, Guid id, CancellationToken ct = default)
    {
        var project = await db.Projects
            .Where(p => p.Id == id && p.OwnerId == ownerId)
            .Select(p => new ProjectResponse(p.Id, p.Name, p.Description, p.CreatedAt))
            .FirstOrDefaultAsync(ct);

        return project ?? throw new NotFoundException($"Project '{id}' was not found.");
    }

    public async Task<ProjectResponse> CreateAsync(Guid ownerId, CreateProjectRequest request, CancellationToken ct = default)
    {
        var project = new Project
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            Description = request.Description,
            OwnerId = ownerId,
            CreatedAt = DateTimeOffset.UtcNow
        };

        db.Projects.Add(project);
        await db.SaveChangesAsync(ct);

        return new ProjectResponse(project.Id, project.Name, project.Description, project.CreatedAt);
    }

    public async Task DeleteAsync(Guid ownerId, Guid id, CancellationToken ct = default)
    {
        var project = await db.Projects
            .FirstOrDefaultAsync(p => p.Id == id && p.OwnerId == ownerId, ct);

        if (project is null)
            throw new NotFoundException($"Project '{id}' was not found.");

        db.Projects.Remove(project);
        await db.SaveChangesAsync(ct);
    }
}
