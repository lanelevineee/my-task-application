using System.ComponentModel.DataAnnotations;

namespace TaskManager.Application.DTOs;

public record CreateTaskDto(
    [Required][MaxLength(200)] string Title,
    string? Description,
    Domain.Enums.TaskStatus Status = Domain.Enums.TaskStatus.Pending,
    Domain.Enums.TaskPriority Priority = Domain.Enums.TaskPriority.Medium,
    DateTime? DueDate = null
);

public record UpdateTaskDto(
    [Required][MaxLength(200)] string Title,
    string? Description,
    Domain.Enums.TaskStatus? Status,
    Domain.Enums.TaskPriority? Priority,
    DateTime? DueDate
);

public record TaskDto(
    Guid Id,
    string Title,
    string? Description,
    Domain.Enums.TaskStatus Status,
    Domain.Enums.TaskPriority Priority,
    DateTime? DueDate,
    DateTime CreatedAt,
    DateTime UpdatedAt
);

public record PagedResult<T>(
    IEnumerable<T> Items,
    int TotalCount,
    int Page,
    int PageSize
)
{
    public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
    public bool HasPreviousPage => Page > 1;
    public bool HasNextPage => Page < TotalPages;
};