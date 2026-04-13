using Microsoft.EntityFrameworkCore;
using TaskService.Domain.Entities;

namespace TaskService.Domain.Interfaces;

public interface IAppDbContext
{
    DbSet<Project> Projects { get; }
    DbSet<TaskItem> Tasks { get; }
    DbSet<Comment> Comments { get; }
    DbSet<Tag> Tags { get; }
    DbSet<User> Users { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
