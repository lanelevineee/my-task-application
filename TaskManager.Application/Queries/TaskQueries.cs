using MediatR;
using TaskManager.Application.DTOs;

namespace TaskManager.Application.Queries;

public record GetTaskByIdQuery(Guid Id) : IRequest<TaskDto?>;

public record GetAllTasksQuery(
    string? Status = null,
    string? Priority = null,
    int Page = 1,
    int PageSize = 10
) : IRequest<PagedResult<TaskDto>>;