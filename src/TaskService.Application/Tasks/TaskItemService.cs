using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TaskService.Application.Common;
using TaskService.Application.Tasks.Dto;
using TaskService.Domain.Entities;
using TaskService.Domain.Interfaces;

namespace TaskService.Application.Tasks;

public class TaskItemService : ITaskItemService
{
    private readonly IAppDbContext _context;
    private readonly ILogger<TaskItemService> _logger;

    public TaskItemService(IAppDbContext context, ILogger<TaskItemService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<PagedResult<TaskItemResponse>> GetAllAsync(
        TaskItemFilter filter,
        CancellationToken ct = default)
    {
        if (filter.Page <= 0) filter.Page = 1;
        if (filter.CountOnPage <= 0) filter.CountOnPage = 20;

        _logger.LogInformation(
            "Запрос списка задач: страница {Page}, размер {CountOnPage}, проект {ProjectId}, статус {Status}, приоритет {Priority}",
            filter.Page, filter.CountOnPage, filter.ProjectId, filter.Status, filter.Priority);

        var query = _context.Tasks.AsQueryable();

        if (filter.ProjectId.HasValue)
            query = query.Where(t => t.ProjectId == filter.ProjectId.Value);

        if (filter.Status.HasValue)
            query = query.Where(t => t.Status == filter.Status.Value);

        if (filter.Priority.HasValue)
            query = query.Where(t => t.Priority == filter.Priority.Value);

        if (filter.AssigneeId.HasValue)
            query = query.Where(t => t.AssigneeId == filter.AssigneeId.Value);

        var skip = (filter.Page - 1) * filter.CountOnPage;

        var total = await query.CountAsync(ct);

        var items = await query
            .OrderByDescending(t => t.CreatedAt)
            .Skip(skip)
            .Take(filter.CountOnPage)
            .Select(t => new TaskItemResponse
            {
                Id = t.Id,
                Title = t.Title,
                Description = t.Description,
                Status = t.Status,
                Priority = t.Priority,
                CreatedAt = t.CreatedAt,
                UpdatedAt = t.UpdatedAt,
                DueDate = t.DueDate,
                ProjectId = t.ProjectId,
                AssigneeId = t.AssigneeId
            })
            .ToListAsync(ct);

        return new PagedResult<TaskItemResponse>
        {
            ObjectCount = total,
            RequestCount = filter.CountOnPage,
            CurrentPage = filter.Page,
            Items = items
        };
    }

    public async Task<TaskItemResponse?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        _logger.LogInformation("Запрос задачи с id {Id}", id);

        var task = await _context.Tasks
            .Where(t => t.Id == id)
            .Select(t => new TaskItemResponse
            {
                Id = t.Id,
                Title = t.Title,
                Description = t.Description,
                Status = t.Status,
                Priority = t.Priority,
                CreatedAt = t.CreatedAt,
                UpdatedAt = t.UpdatedAt,
                DueDate = t.DueDate,
                ProjectId = t.ProjectId,
                AssigneeId = t.AssigneeId
            })
            .FirstOrDefaultAsync(ct);

        if (task is null)
            _logger.LogWarning("Задача с id {Id} не найдена", id);

        return task;
    }

    public async Task<TaskItemResponse> CreateAsync(
        CreateTaskItemRequest request,
        CancellationToken ct = default)
    {
        var projectExists = await _context.Projects
            .AnyAsync(p => p.Id == request.ProjectId, ct);

        if (!projectExists)
        {
            _logger.LogWarning(
                "Попытка создать задачу для несуществующего проекта с id {ProjectId}",
                request.ProjectId);
            throw new InvalidOperationException(
                $"Проект с id {request.ProjectId} не найден.");
        }

        var now = DateTime.UtcNow;

        var taskItem = new TaskItem
        {
            Id = Guid.NewGuid(),
            Title = request.Title,
            Description = request.Description,
            Status = Domain.Enums.TaskItemStatus.Backlog,
            Priority = request.Priority,
            CreatedAt = now,
            UpdatedAt = now,
            DueDate = request.DueDate,
            ProjectId = request.ProjectId,
            AssigneeId = request.AssigneeId
        };

        _context.Tasks.Add(taskItem);
        await _context.SaveChangesAsync(ct);

        _logger.LogInformation(
            "Создана задача '{Title}' с id {Id} в проекте {ProjectId}",
            taskItem.Title, taskItem.Id, taskItem.ProjectId);

        return new TaskItemResponse
        {
            Id = taskItem.Id,
            Title = taskItem.Title,
            Description = taskItem.Description,
            Status = taskItem.Status,
            Priority = taskItem.Priority,
            CreatedAt = taskItem.CreatedAt,
            UpdatedAt = taskItem.UpdatedAt,
            DueDate = taskItem.DueDate,
            ProjectId = taskItem.ProjectId,
            AssigneeId = taskItem.AssigneeId
        };
    }

    public async Task<TaskItemResponse?> UpdateAsync(
        Guid id,
        UpdateTaskItemRequest request,
        CancellationToken ct = default)
    {
        var taskItem = await _context.Tasks
            .FirstOrDefaultAsync(t => t.Id == id, ct);

        if (taskItem is null)
        {
            _logger.LogWarning("Задача с id {Id} не найдена при обновлении", id);
            return null;
        }

        if (request.Title is not null)
            taskItem.Title = request.Title;

        if (request.Description is not null)
            taskItem.Description = request.Description;

        if (request.Status.HasValue)
            taskItem.Status = request.Status.Value;

        if (request.Priority.HasValue)
            taskItem.Priority = request.Priority.Value;

        if (request.DueDate.HasValue)
            taskItem.DueDate = request.DueDate.Value;

        if (request.AssigneeId.HasValue)
            taskItem.AssigneeId = request.AssigneeId.Value;

        taskItem.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync(ct);

        _logger.LogInformation("Задача с id {Id} обновлена", id);

        return new TaskItemResponse
        {
            Id = taskItem.Id,
            Title = taskItem.Title,
            Description = taskItem.Description,
            Status = taskItem.Status,
            Priority = taskItem.Priority,
            CreatedAt = taskItem.CreatedAt,
            UpdatedAt = taskItem.UpdatedAt,
            DueDate = taskItem.DueDate,
            ProjectId = taskItem.ProjectId,
            AssigneeId = taskItem.AssigneeId
        };
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken ct = default)
    {
        var taskItem = await _context.Tasks
            .FirstOrDefaultAsync(t => t.Id == id, ct);

        if (taskItem is null)
        {
            _logger.LogWarning("Задача с id {Id} не найдена при удалении", id);
            return false;
        }

        _context.Tasks.Remove(taskItem);
        await _context.SaveChangesAsync(ct);

        _logger.LogInformation("Задача с id {Id} удалена", id);
        return true;
    }
}
