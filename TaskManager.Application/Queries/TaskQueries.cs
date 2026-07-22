using MediatR;
using TaskManager.Application.DTOs;

namespace TaskManager.Application.Queries;

public record GetTaskByIdQuery(Guid Id) : IRequest<TaskDto?>;

public record GetAllTasksQuery(
    Domain.Enums.TaskStatus? Status = null,
    Domain.Enums.TaskPriority? Priority = null,
    int Page = 1,
    int PageSize = 10
) : IRequest<PagedResult<TaskDto>>;