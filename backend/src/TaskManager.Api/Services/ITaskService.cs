using TaskManager.Api.Dtos;
using TaskManager.Api.Entities;

namespace TaskManager.Api.Services;

public interface ITaskService
{
    Task<IReadOnlyList<TaskResponse>> GetAllAsync(
        Guid ownerId,
        Guid projectId,
        TaskItemStatus? status,
        TaskPriority? priority,
        CancellationToken ct = default);

    Task<TaskResponse> CreateAsync(Guid ownerId, Guid projectId, CreateTaskRequest request, CancellationToken ct = default);
    Task<TaskResponse> UpdateAsync(Guid ownerId, Guid projectId, Guid id, UpdateTaskRequest request, CancellationToken ct = default);
    Task DeleteAsync(Guid ownerId, Guid projectId, Guid id, CancellationToken ct = default);
}
