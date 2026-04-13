using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TaskService.Application.Comments.Dto;
using TaskService.Domain.Entities;
using TaskService.Domain.Interfaces;

namespace TaskService.Application.Comments;

public class CommentService : ICommentService
{
    private readonly IAppDbContext _context;
    private readonly ILogger<CommentService> _logger;

    public CommentService(IAppDbContext context, ILogger<CommentService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<IReadOnlyList<CommentResponse>> GetByTaskIdAsync(
        Guid taskItemId,
        CancellationToken ct = default)
    {
        _logger.LogInformation("Запрос комментариев для задачи с id {TaskItemId}", taskItemId);

        var comments = await _context.Comments
            .Where(c => c.TaskItemId == taskItemId)
            .OrderBy(c => c.CreatedAt)
            .Select(c => new CommentResponse
            {
                Id = c.Id,
                Content = c.Content,
                CreatedAt = c.CreatedAt,
                UpdatedAt = c.UpdatedAt,
                TaskItemId = c.TaskItemId,
                AuthorId = c.AuthorId
            })
            .ToListAsync(ct);

        return comments;
    }

    public async Task<CommentResponse?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        _logger.LogInformation("Запрос комментария с id {Id}", id);

        var comment = await _context.Comments
            .Where(c => c.Id == id)
            .Select(c => new CommentResponse
            {
                Id = c.Id,
                Content = c.Content,
                CreatedAt = c.CreatedAt,
                UpdatedAt = c.UpdatedAt,
                TaskItemId = c.TaskItemId,
                AuthorId = c.AuthorId
            })
            .FirstOrDefaultAsync(ct);

        if (comment is null)
            _logger.LogWarning("Комментарий с id {Id} не найден", id);

        return comment;
    }

    public async Task<CommentResponse> CreateAsync(
        CreateCommentRequest request,
        CancellationToken ct = default)
    {
        var taskExists = await _context.Tasks
            .AnyAsync(t => t.Id == request.TaskItemId, ct);

        if (!taskExists)
        {
            _logger.LogWarning(
                "Попытка добавить комментарий к несуществующей задаче с id {TaskItemId}",
                request.TaskItemId);
            throw new InvalidOperationException(
                $"Задача с id {request.TaskItemId} не найдена.");
        }

        var now = DateTime.UtcNow;

        var comment = new Comment
        {
            Id = Guid.NewGuid(),
            Content = request.Content,
            CreatedAt = now,
            UpdatedAt = now,
            TaskItemId = request.TaskItemId,
            AuthorId = request.AuthorId
        };

        _context.Comments.Add(comment);
        await _context.SaveChangesAsync(ct);

        _logger.LogInformation(
            "Добавлен комментарий с id {Id} к задаче {TaskItemId}",
            comment.Id, comment.TaskItemId);

        return new CommentResponse
        {
            Id = comment.Id,
            Content = comment.Content,
            CreatedAt = comment.CreatedAt,
            UpdatedAt = comment.UpdatedAt,
            TaskItemId = comment.TaskItemId,
            AuthorId = comment.AuthorId
        };
    }

    public async Task<CommentResponse?> UpdateAsync(
        Guid id,
        UpdateCommentRequest request,
        CancellationToken ct = default)
    {
        var comment = await _context.Comments
            .FirstOrDefaultAsync(c => c.Id == id, ct);

        if (comment is null)
        {
            _logger.LogWarning("Комментарий с id {Id} не найден при обновлении", id);
            return null;
        }

        comment.Content = request.Content;
        comment.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync(ct);

        _logger.LogInformation("Комментарий с id {Id} обновлён", id);

        return new CommentResponse
        {
            Id = comment.Id,
            Content = comment.Content,
            CreatedAt = comment.CreatedAt,
            UpdatedAt = comment.UpdatedAt,
            TaskItemId = comment.TaskItemId,
            AuthorId = comment.AuthorId
        };
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken ct = default)
    {
        var comment = await _context.Comments
            .FirstOrDefaultAsync(c => c.Id == id, ct);

        if (comment is null)
        {
            _logger.LogWarning("Комментарий с id {Id} не найден при удалении", id);
            return false;
        }

        _context.Comments.Remove(comment);
        await _context.SaveChangesAsync(ct);

        _logger.LogInformation("Комментарий с id {Id} удалён", id);
        return true;
    }
}
