using TaskService.Application.Common;
using TaskService.Application.Projects.Dto;

namespace TaskService.Application.Projects;

public interface IProjectService
{
    Task<PagedResult<ProjectResponse>> GetAllAsync(
        int page = 1,
        int countOnPage = 20,
        CancellationToken ct = default);

    Task<ProjectResponse?> GetByIdAsync(Guid id, CancellationToken ct = default);

    Task<ProjectResponse> CreateAsync(CreateProjectRequest request, CancellationToken ct = default);

    Task<ProjectResponse?> UpdateAsync(
        Guid id,
        UpdateProjectRequest request,
        CancellationToken ct = default);

    Task<bool> DeleteAsync(Guid id, CancellationToken ct = default);
}
