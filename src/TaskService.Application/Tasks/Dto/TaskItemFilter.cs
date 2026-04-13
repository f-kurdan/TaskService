using TaskService.Domain.Enums;

namespace TaskService.Application.Tasks.Dto;

public class TaskItemFilter
{
    public Guid? ProjectId { get; set; }
    public TaskItemStatus? Status { get; set; }
    public Priority? Priority { get; set; }
    public Guid? AssigneeId { get; set; }
    public int Page { get; set; } = 1;
    public int CountOnPage { get; set; } = 20;
}
