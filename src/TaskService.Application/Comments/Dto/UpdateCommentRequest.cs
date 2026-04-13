using System.ComponentModel.DataAnnotations;

namespace TaskService.Application.Comments.Dto;

public class UpdateCommentRequest
{
    [Required]
    [MaxLength(5000)]
    public string Content { get; set; } = string.Empty;
}
