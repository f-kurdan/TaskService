using TaskService.Application.Comments.Dto;

namespace TaskService.Application.Comments;

public interface ICommentService
{
    Task<IReadOnlyList<CommentResponse>> GetByTaskIdAsync(
        Guid taskItemId,
        CancellationToken ct = default);

    Task<CommentResponse?> GetByIdAsync(Guid id, CancellationToken ct = default);

    Task<CommentResponse> CreateAsync(CreateCommentRequest request, CancellationToken ct = default);

    Task<CommentResponse?> UpdateAsync(
        Guid id,
        UpdateCommentRequest request,
        CancellationToken ct = default);

    Task<bool> DeleteAsync(Guid id, CancellationToken ct = default);
}
