using System.ComponentModel.DataAnnotations;

namespace TaskService.Application.Projects.Dto;

public class UpdateProjectRequest
{
    [MaxLength(200)]
    public string? Name { get; set; }

    [MaxLength(2000)]
    public string? Description { get; set; }
}
