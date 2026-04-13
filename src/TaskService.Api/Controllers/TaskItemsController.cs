using Microsoft.AspNetCore.Mvc;
using TaskService.Application.Tasks;
using TaskService.Application.Tasks.Dto;

namespace TaskService.Api.Controllers;

[ApiController]
[Route("api/tasks")]
public class TaskItemsController : ControllerBase
{
    private readonly ITaskItemService _taskItemService;

    public TaskItemsController(ITaskItemService taskItemService)
    {
        _taskItemService = taskItemService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] TaskItemFilter filter, CancellationToken ct)
    {
        var result = await _taskItemService.GetAllAsync(filter, ct);
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
    {
        var task = await _taskItemService.GetByIdAsync(id, ct);
        if (task is null)
            return NotFound();
        return Ok(task);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateTaskItemRequest request, CancellationToken ct)
    {
        var task = await _taskItemService.CreateAsync(request, ct);
        return CreatedAtAction(nameof(GetById), new { id = task.Id }, task);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(
        Guid id,
        [FromBody] UpdateTaskItemRequest request,
        CancellationToken ct)
    {
        var task = await _taskItemService.UpdateAsync(id, request, ct);
        if (task is null)
            return NotFound();
        return Ok(task);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        var deleted = await _taskItemService.DeleteAsync(id, ct);
        if (!deleted)
            return NotFound();
        return NoContent();
    }
}
