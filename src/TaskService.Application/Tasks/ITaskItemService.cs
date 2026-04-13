using TaskService.Application.Common;
using TaskService.Application.Tasks.Dto;

namespace TaskService.Application.Tasks;

public interface ITaskItemService
{
    Task<PagedResult<TaskItemResponse>> GetAllAsync(
        TaskItemFilter filter,
        CancellationToken ct = default);

    Task<TaskItemResponse?> GetByIdAsync(Guid id, CancellationToken ct = default);

    Task<TaskItemResponse> CreateAsync(CreateTaskItemRequest request, CancellationToken ct = default);

    Task<TaskItemResponse?> UpdateAsync(
        Guid id,
        UpdateTaskItemRequest request,
        CancellationToken ct = default);

    Task<bool> DeleteAsync(Guid id, CancellationToken ct = default);
}
