using Microsoft.AspNetCore.Mvc;
using TaskManager.Api.Common;
using TaskManager.Api.Dtos;
using TaskManager.Api.Services;

namespace TaskManager.Api.Controllers;

[ApiController]
[Route("api/projects")]
public class ProjectsController(IProjectService projectService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<ProjectResponse>>> GetAll(CancellationToken ct)
    {
        var projects = await projectService.GetAllAsync(DevUser.PlaceholderOwnerId, ct);
        return Ok(projects);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ProjectResponse>> GetById(Guid id, CancellationToken ct)
    {
        var project = await projectService.GetByIdAsync(DevUser.PlaceholderOwnerId, id, ct);
        return Ok(project);
    }

    [HttpPost]
    public async Task<ActionResult<ProjectResponse>> Create(CreateProjectRequest request, CancellationToken ct)
    {
        var project = await projectService.CreateAsync(DevUser.PlaceholderOwnerId, request, ct);
        return CreatedAtAction(nameof(GetById), new { id = project.Id }, project);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        await projectService.DeleteAsync(DevUser.PlaceholderOwnerId, id, ct);
        return NoContent();
    }
}
