using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TaskService.Application.Common;
using TaskService.Application.Projects.Dto;
using TaskService.Domain.Entities;
using TaskService.Domain.Interfaces;

namespace TaskService.Application.Projects;

public class ProjectService : IProjectService
{
    private readonly IAppDbContext _context;
    private readonly ILogger<ProjectService> _logger;

    public ProjectService(IAppDbContext context, ILogger<ProjectService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<PagedResult<ProjectResponse>> GetAllAsync(
        int page = 1,
        int countOnPage = 20,
        CancellationToken ct = default)
    {
        if (page <= 0) page = 1;
        if (countOnPage <= 0) countOnPage = 20;

        _logger.LogInformation(
            "Запрос списка проектов: страница {Page}, размер {CountOnPage}",
            page, countOnPage);

        var skip = (page - 1) * countOnPage;

        var total = await _context.Projects.CountAsync(ct);

        var items = await _context.Projects
            .OrderBy(p => p.Name)
            .Skip(skip)
            .Take(countOnPage)
            .Select(p => new ProjectResponse
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                CreatedAt = p.CreatedAt,
                UpdatedAt = p.UpdatedAt,
                TaskCount = p.Tasks.Count
            })
            .ToListAsync(ct);

        return new PagedResult<ProjectResponse>
        {
            ObjectCount = total,
            RequestCount = countOnPage,
            CurrentPage = page,
            Items = items
        };
    }

    public async Task<ProjectResponse?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        _logger.LogInformation("Запрос проекта с id {Id}", id);

        var project = await _context.Projects
            .Where(p => p.Id == id)
            .Select(p => new ProjectResponse
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                CreatedAt = p.CreatedAt,
                UpdatedAt = p.UpdatedAt,
                TaskCount = p.Tasks.Count
            })
            .FirstOrDefaultAsync(ct);

        if (project is null)
            _logger.LogWarning("Проект с id {Id} не найден", id);

        return project;
    }

    public async Task<ProjectResponse> CreateAsync(
        CreateProjectRequest request,
        CancellationToken ct = default)
    {
        var now = DateTime.UtcNow;

        var project = new Project
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            Description = request.Description,
            CreatedAt = now,
            UpdatedAt = now
        };

        _context.Projects.Add(project);
        await _context.SaveChangesAsync(ct);

        _logger.LogInformation("Создан проект '{Name}' с id {Id}", project.Name, project.Id);

        return new ProjectResponse
        {
            Id = project.Id,
            Name = project.Name,
            Description = project.Description,
            CreatedAt = project.CreatedAt,
            UpdatedAt = project.UpdatedAt,
            TaskCount = 0
        };
    }

    public async Task<ProjectResponse?> UpdateAsync(
        Guid id,
        UpdateProjectRequest request,
        CancellationToken ct = default)
    {
        var project = await _context.Projects
            .Include(p => p.Tasks)
            .FirstOrDefaultAsync(p => p.Id == id, ct);

        if (project is null)
        {
            _logger.LogWarning("Проект с id {Id} не найден при обновлении", id);
            return null;
        }

        if (request.Name is not null)
            project.Name = request.Name;

        if (request.Description is not null)
            project.Description = request.Description;

        project.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync(ct);

        _logger.LogInformation("Проект с id {Id} обновлён", id);

        return new ProjectResponse
        {
            Id = project.Id,
            Name = project.Name,
            Description = project.Description,
            CreatedAt = project.CreatedAt,
            UpdatedAt = project.UpdatedAt,
            TaskCount = project.Tasks.Count
        };
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken ct = default)
    {
        var project = await _context.Projects
            .FirstOrDefaultAsync(p => p.Id == id, ct);

        if (project is null)
        {
            _logger.LogWarning("Проект с id {Id} не найден при удалении", id);
            return false;
        }

        _context.Projects.Remove(project);
        await _context.SaveChangesAsync(ct);

        _logger.LogInformation("Проект с id {Id} удалён", id);
        return true;
    }
}
