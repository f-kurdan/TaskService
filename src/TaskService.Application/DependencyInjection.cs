using Microsoft.Extensions.DependencyInjection;
using TaskService.Application.Comments;
using TaskService.Application.Projects;
using TaskService.Application.Tasks;

namespace TaskService.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IProjectService, ProjectService>();
        services.AddScoped<ITaskItemService, TaskItemService>();
        services.AddScoped<ICommentService, CommentService>();

        return services;
    }
}
