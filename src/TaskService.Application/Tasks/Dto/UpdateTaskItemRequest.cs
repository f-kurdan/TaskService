using System.ComponentModel.DataAnnotations;
using TaskService.Domain.Enums;

namespace TaskService.Application.Tasks.Dto;

public class UpdateTaskItemRequest
{
    [MaxLength(500)]
    public string? Title { get; set; }

    [MaxLength(5000)]
    public string? Description { get; set; }

    public TaskItemStatus? Status { get; set; }

    public Priority? Priority { get; set; }

    public DateTime? DueDate { get; set; }

    public Guid? AssigneeId { get; set; }
}
