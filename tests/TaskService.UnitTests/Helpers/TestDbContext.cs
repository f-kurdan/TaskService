using Microsoft.EntityFrameworkCore;
using TaskService.Domain.Entities;
using TaskService.Domain.Interfaces;

namespace TaskService.UnitTests.Helpers;

public class TestDbContext : DbContext, IAppDbContext
{
    public DbSet<Project> Projects { get; set; } = null!;
    public DbSet<TaskItem> Tasks { get; set; } = null!;
    public DbSet<Comment> Comments { get; set; } = null!;
    public DbSet<Tag> Tags { get; set; } = null!;
    public DbSet<User> Users { get; set; } = null!;

    public TestDbContext(DbContextOptions<TestDbContext> options) : base(options) { }

    public static TestDbContext Create(string dbName)
    {
        var options = new DbContextOptionsBuilder<TestDbContext>()
            .UseInMemoryDatabase(dbName)
            .Options;

        return new TestDbContext(options);
    }
}
