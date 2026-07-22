using MediatR;
using TaskManager.Application.DTOs;
using TaskManager.Application.Interfaces;
using TaskManager.Application.Queries;
using TaskEntity = TaskManager.Domain.Entities.Task;

namespace TaskManager.Application.Handlers.Queries;

public class GetTaskByIdQueryHandler : IRequestHandler<GetTaskByIdQuery, TaskDto?>
{
    private readonly ITaskRepository _taskRepository;

    public GetTaskByIdQueryHandler(ITaskRepository taskRepository)
    {
        _taskRepository = taskRepository;
    }

    public async Task<TaskDto?> Handle(GetTaskByIdQuery request, CancellationToken cancellationToken)
    {
        var task = await _taskRepository.GetByIdAsync(request.Id, cancellationToken);
        if (task is null) return null;

        return new TaskDto(
            task.Id,
            task.Title,
            task.Description,
            task.Status,
            task.Priority,
            task.DueDate,
            task.CreatedAt,
            task.UpdatedAt
        );
    }
}

public class GetAllTasksQueryHandler : IRequestHandler<GetAllTasksQuery, PagedResult<TaskDto>>
{
    private readonly ITaskRepository _taskRepository;

    public GetAllTasksQueryHandler(ITaskRepository taskRepository)
    {
        _taskRepository = taskRepository;
    }

    public async Task<PagedResult<TaskDto>> Handle(GetAllTasksQuery request, CancellationToken cancellationToken)
    {
        var (tasks, totalCount) = await _taskRepository.GetAllAsync(
            request.Status,
            request.Priority,
            request.Page,
            request.PageSize,
            cancellationToken
        );

        var taskDtos = tasks.Select(t => new TaskDto(
            t.Id,
            t.Title,
            t.Description,
            t.Status,
            t.Priority,
            t.DueDate,
            t.CreatedAt,
            t.UpdatedAt
        ));

        return new PagedResult<TaskDto>(taskDtos, totalCount, request.Page, request.PageSize);
    }
}