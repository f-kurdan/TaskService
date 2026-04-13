namespace TaskService.Application.Common;

public class PagedResult<T>
{
    public int ObjectCount { get; init; }
    public int RequestCount { get; init; }
    public int CurrentPage { get; init; }
    public IReadOnlyList<T> Items { get; init; } = [];
}
