using Microsoft.Extensions.Logging;
using Moq;
using TaskService.Application.Projects;
using TaskService.Application.Projects.Dto;
using TaskService.Domain.Entities;
using TaskService.UnitTests.Helpers;
using Xunit;

namespace TaskService.UnitTests.Projects;

public class ProjectServiceTests : IDisposable
{
    private readonly TestDbContext _db;
    private readonly ProjectService _sut;

    public ProjectServiceTests()
    {
        _db = TestDbContext.Create(Guid.NewGuid().ToString());
        var logger = new Mock<ILogger<ProjectService>>().Object;
        _sut = new ProjectService(_db, logger);
    }

    public void Dispose() => _db.Dispose();

    // ─── GetAll ────────────────────────────────────────────────────────────────

    [Fact]
    public async Task GetAllAsync_SkipTakeAndMetadata_AreCorrect()
    {
        // Arrange: 25 проектов с именами, сортируемыми по алфавиту
        for (var i = 1; i <= 25; i++)
        {
            _db.Projects.Add(new Project
            {
                Id = Guid.NewGuid(),
                Name = $"Project {i:D2}",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            });
        }
        await _db.SaveChangesAsync();

        // Act: страница 2, 10 записей на странице → skip=10, take=10
        var result = await _sut.GetAllAsync(page: 2, countOnPage: 10);

        // Assert: метаданные
        Assert.Equal(25, result.ObjectCount);
        Assert.Equal(10, result.RequestCount);
        Assert.Equal(2, result.CurrentPage);

        // Assert: пропущены первые 10, взяты следующие 10
        Assert.Equal(10, result.Items.Count);
        Assert.Equal("Project 11", result.Items.First().Name);
        Assert.Equal("Project 20", result.Items.Last().Name);
    }

    [Fact]
    public async Task GetAllAsync_NegativePage_TreatedAsFirstPage()
    {
        _db.Projects.Add(new Project
        {
            Id = Guid.NewGuid(),
            Name = "Alpha",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        });
        await _db.SaveChangesAsync();

        // page <= 0 должен сбрасываться в 1
        var result = await _sut.GetAllAsync(page: -1, countOnPage: 5);

        Assert.Equal(1, result.CurrentPage);
        Assert.Single(result.Items);
    }

    [Fact]
    public async Task GetAllAsync_ZeroCountOnPage_DefaultsToTwenty()
    {
        var result = await _sut.GetAllAsync(page: 1, countOnPage: 0);

        // countOnPage <= 0 должен сбрасываться в 20
        Assert.Equal(20, result.RequestCount);
    }

    // ─── GetById ───────────────────────────────────────────────────────────────

    [Fact]
    public async Task GetByIdAsync_ExistingProject_ReturnsResponse()
    {
        var id = Guid.NewGuid();
        _db.Projects.Add(new Project
        {
            Id = id,
            Name = "Test Project",
            Description = "Описание",
            CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc),
            UpdatedAt = new DateTime(2026, 1, 2, 0, 0, 0, DateTimeKind.Utc)
        });
        await _db.SaveChangesAsync();

        var result = await _sut.GetByIdAsync(id);

        Assert.NotNull(result);
        Assert.Equal(id, result.Id);
        Assert.Equal("Test Project", result.Name);
        Assert.Equal("Описание", result.Description);
        Assert.Equal(0, result.TaskCount);
    }

    [Fact]
    public async Task GetByIdAsync_MissingProject_ReturnsNull()
    {
        var result = await _sut.GetByIdAsync(Guid.NewGuid());

        Assert.Null(result);
    }

    // ─── Create ────────────────────────────────────────────────────────────────

    [Fact]
    public async Task CreateAsync_HappyPath_ReturnsPersistedProject()
    {
        var request = new CreateProjectRequest
        {
            Name = "Новый проект",
            Description = "Тестовое описание"
        };

        var result = await _sut.CreateAsync(request);

        Assert.NotEqual(Guid.Empty, result.Id);
        Assert.Equal("Новый проект", result.Name);
        Assert.Equal("Тестовое описание", result.Description);
        Assert.Equal(0, result.TaskCount);

        // Запись действительно сохранена в БД
        var saved = await _db.Projects.FindAsync(result.Id);
        Assert.NotNull(saved);
        Assert.Equal("Новый проект", saved.Name);
    }

    // ─── Update ────────────────────────────────────────────────────────────────

    [Fact]
    public async Task UpdateAsync_ExistingProject_ReturnsUpdatedResponse()
    {
        var id = Guid.NewGuid();
        _db.Projects.Add(new Project
        {
            Id = id,
            Name = "Старое название",
            Description = "Старое описание",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        });
        await _db.SaveChangesAsync();

        var request = new UpdateProjectRequest
        {
            Name = "Новое название",
            Description = "Новое описание"
        };

        var result = await _sut.UpdateAsync(id, request);

        Assert.NotNull(result);
        Assert.Equal("Новое название", result.Name);
        Assert.Equal("Новое описание", result.Description);
    }

    [Fact]
    public async Task UpdateAsync_MissingProject_ReturnsNull()
    {
        var result = await _sut.UpdateAsync(Guid.NewGuid(), new UpdateProjectRequest { Name = "X" });

        Assert.Null(result);
    }

    // ─── Delete ────────────────────────────────────────────────────────────────

    [Fact]
    public async Task DeleteAsync_ExistingProject_ReturnsTrueAndRemovesEntity()
    {
        var id = Guid.NewGuid();
        _db.Projects.Add(new Project
        {
            Id = id,
            Name = "На удаление",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        });
        await _db.SaveChangesAsync();

        var deleted = await _sut.DeleteAsync(id);

        Assert.True(deleted);
        Assert.Null(await _db.Projects.FindAsync(id));
    }

    [Fact]
    public async Task DeleteAsync_MissingProject_ReturnsFalse()
    {
        var deleted = await _sut.DeleteAsync(Guid.NewGuid());

        Assert.False(deleted);
    }
}
