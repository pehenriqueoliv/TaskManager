using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskManager.Api.Common;
using TaskManager.Api.Dtos;
using TaskManager.Api.Entities;
using TaskManager.Api.Services;

namespace TaskManager.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/projects/{projectId:guid}/tasks")]
public class TasksController(ITaskService taskService, ICurrentUser currentUser) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<TaskResponse>>> GetAll(
        Guid projectId,
        [FromQuery] TaskItemStatus? status,
        [FromQuery] TaskPriority? priority,
        CancellationToken ct)
    {
        var tasks = await taskService.GetAllAsync(currentUser.Id, projectId, status, priority, ct);
        return Ok(tasks);
    }

    [HttpPost]
    public async Task<ActionResult<TaskResponse>> Create(Guid projectId, CreateTaskRequest request, CancellationToken ct)
    {
        var task = await taskService.CreateAsync(currentUser.Id, projectId, request, ct);
        return CreatedAtAction(nameof(GetAll), new { projectId }, task);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<TaskResponse>> Update(Guid projectId, Guid id, UpdateTaskRequest request, CancellationToken ct)
    {
        var task = await taskService.UpdateAsync(currentUser.Id, projectId, id, request, ct);
        return Ok(task);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid projectId, Guid id, CancellationToken ct)
    {
        await taskService.DeleteAsync(currentUser.Id, projectId, id, ct);
        return NoContent();
    }
}
