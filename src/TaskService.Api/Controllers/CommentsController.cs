using Microsoft.AspNetCore.Mvc;
using TaskService.Application.Comments;
using TaskService.Application.Comments.Dto;

namespace TaskService.Api.Controllers;

[ApiController]
[Route("api/tasks/{taskItemId:guid}/comments")]
public class CommentsController : ControllerBase
{
    private readonly ICommentService _commentService;

    public CommentsController(ICommentService commentService)
    {
        _commentService = commentService;
    }

    [HttpGet]
    public async Task<IActionResult> GetByTaskId(Guid taskItemId, CancellationToken ct)
    {
        var comments = await _commentService.GetByTaskIdAsync(taskItemId, ct);
        return Ok(comments);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid taskItemId, Guid id, CancellationToken ct)
    {
        var comment = await _commentService.GetByIdAsync(id, ct);
        if (comment is null)
            return NotFound();
        return Ok(comment);
    }

    [HttpPost]
    public async Task<IActionResult> Create(
        Guid taskItemId,
        [FromBody] CreateCommentRequest request,
        CancellationToken ct)
    {
        request.TaskItemId = taskItemId;
        var comment = await _commentService.CreateAsync(request, ct);
        return CreatedAtAction(nameof(GetById), new { taskItemId, id = comment.Id }, comment);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(
        Guid taskItemId,
        Guid id,
        [FromBody] UpdateCommentRequest request,
        CancellationToken ct)
    {
        var comment = await _commentService.UpdateAsync(id, request, ct);
        if (comment is null)
            return NotFound();
        return Ok(comment);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid taskItemId, Guid id, CancellationToken ct)
    {
        var deleted = await _commentService.DeleteAsync(id, ct);
        if (!deleted)
            return NotFound();
        return NoContent();
    }
}
