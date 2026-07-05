using TaskManager.Api.Dtos;

namespace TaskManager.Api.Services;

public interface IProjectService
{
    Task<IReadOnlyList<ProjectResponse>> GetAllAsync(Guid ownerId, CancellationToken ct = default);
    Task<ProjectResponse> GetByIdAsync(Guid ownerId, Guid id, CancellationToken ct = default);
    Task<ProjectResponse> CreateAsync(Guid ownerId, CreateProjectRequest request, CancellationToken ct = default);
    Task DeleteAsync(Guid ownerId, Guid id, CancellationToken ct = default);
}
