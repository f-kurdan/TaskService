namespace TaskService.Application.Comments.Dto;

public class CommentResponse
{
    public Guid Id { get; init; }
    public string Content { get; init; } = string.Empty;
    public DateTime CreatedAt { get; init; }
    public DateTime UpdatedAt { get; init; }
    public Guid TaskItemId { get; init; }
    public Guid AuthorId { get; init; }
}
