using System.ComponentModel.DataAnnotations;

namespace TaskService.Application.Comments.Dto;

public class CreateCommentRequest
{
    [Required]
    [MaxLength(5000)]
    public string Content { get; set; } = string.Empty;

    [Required]
    public Guid TaskItemId { get; set; }

    [Required]
    public Guid AuthorId { get; set; }
}
