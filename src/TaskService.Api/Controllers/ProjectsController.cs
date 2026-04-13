using Microsoft.AspNetCore.Mvc;
using TaskService.Application.Projects;
using TaskService.Application.Projects.Dto;

namespace TaskService.Api.Controllers;

[ApiController]
[Route("api/projects")]
public class ProjectsController : ControllerBase
{
    private readonly IProjectService _projectService;

    public ProjectsController(IProjectService projectService)
    {
        _projectService = projectService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] int page = 1,
        [FromQuery] int countOnPage = 20,
        CancellationToken ct = default)
    {
        var result = await _projectService.GetAllAsync(page, countOnPage, ct);
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
    {
        var project = await _projectService.GetByIdAsync(id, ct);
        if (project is null)
            return NotFound();
        return Ok(project);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateProjectRequest request, CancellationToken ct)
    {
        var project = await _projectService.CreateAsync(request, ct);
        return CreatedAtAction(nameof(GetById), new { id = project.Id }, project);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(
        Guid id,
        [FromBody] UpdateProjectRequest request,
        CancellationToken ct)
    {
        var project = await _projectService.UpdateAsync(id, request, ct);
        if (project is null)
            return NotFound();
        return Ok(project);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        var deleted = await _projectService.DeleteAsync(id, ct);
        if (!deleted)
            return NotFound();
        return NoContent();
    }
}
