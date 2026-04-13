using TaskService.Domain.Enums;

namespace TaskService.Application.Tasks.Dto;

public class TaskItemResponse
{
    public Guid Id { get; init; }
    public string Title { get; init; } = string.Empty;
    public string? Description { get; init; }
    public TaskItemStatus Status { get; init; }
    public Priority Priority { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime UpdatedAt { get; init; }
    public DateTime? DueDate { get; init; }
    public Guid ProjectId { get; init; }
    public Guid? AssigneeId { get; init; }
}
