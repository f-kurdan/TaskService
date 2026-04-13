using System.ComponentModel.DataAnnotations;
using TaskService.Domain.Enums;

namespace TaskService.Application.Tasks.Dto;

public class CreateTaskItemRequest
{
    [Required]
    [MaxLength(500)]
    public string Title { get; set; } = string.Empty;

    [MaxLength(5000)]
    public string? Description { get; set; }

    [Required]
    public Guid ProjectId { get; set; }

    public Priority Priority { get; set; } = Priority.Medium;

    public DateTime? DueDate { get; set; }

    public Guid? AssigneeId { get; set; }
}
