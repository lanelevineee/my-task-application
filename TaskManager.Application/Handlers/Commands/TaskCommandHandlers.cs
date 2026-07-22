using MediatR;
using TaskManager.Application.Commands;
using TaskManager.Application.DTOs;
using TaskManager.Application.Interfaces;
using TaskEntity = TaskManager.Domain.Entities.Task;

namespace TaskManager.Application.Handlers.Commands;

public class CreateTaskCommandHandler : IRequestHandler<CreateTaskCommand, TaskDto>
{
    private readonly ITaskRepository _taskRepository;

    public CreateTaskCommandHandler(ITaskRepository taskRepository)
    {
        _taskRepository = taskRepository;
    }

    public async Task<TaskDto> Handle(CreateTaskCommand request, CancellationToken cancellationToken)
    {
        var task = new TaskEntity
        {
            Id = Guid.NewGuid(),
            Title = request.Task.Title,
            Description = request.Task.Description,
            Status = request.Task.Status,
            Priority = request.Task.Priority,
            DueDate = request.Task.DueDate,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        var created = await _taskRepository.CreateAsync(task, cancellationToken);

        return new TaskDto(
            created.Id,
            created.Title,
            created.Description,
            created.Status,
            created.Priority,
            created.DueDate,
            created.CreatedAt,
            created.UpdatedAt
        );
    }
}

public class UpdateTaskCommandHandler : IRequestHandler<UpdateTaskCommand, TaskDto>
{
    private readonly ITaskRepository _taskRepository;

    public UpdateTaskCommandHandler(ITaskRepository taskRepository)
    {
        _taskRepository = taskRepository;
    }

    public async Task<TaskDto> Handle(UpdateTaskCommand request, CancellationToken cancellationToken)
    {
        var existing = await _taskRepository.GetByIdAsync(request.Id, cancellationToken);
        if (existing is null)
            throw new KeyNotFoundException($"Task with ID {request.Id} not found");

        existing.Title = request.Task.Title;
        existing.Description = request.Task.Description;
        existing.Status = request.Task.Status ?? existing.Status;
        existing.Priority = request.Task.Priority ?? existing.Priority;
        existing.DueDate = request.Task.DueDate ?? existing.DueDate;
        existing.UpdatedAt = DateTime.UtcNow;

        var updated = await _taskRepository.UpdateAsync(existing, cancellationToken);

        return new TaskDto(
            updated.Id,
            updated.Title,
            updated.Description,
            updated.Status,
            updated.Priority,
            updated.DueDate,
            updated.CreatedAt,
            updated.UpdatedAt
        );
    }
}

public class DeleteTaskCommandHandler : IRequestHandler<DeleteTaskCommand, bool>
{
    private readonly ITaskRepository _taskRepository;

    public DeleteTaskCommandHandler(ITaskRepository taskRepository)
    {
        _taskRepository = taskRepository;
    }

    public async Task<bool> Handle(DeleteTaskCommand request, CancellationToken cancellationToken)
    {
        return await _taskRepository.DeleteAsync(request.Id, cancellationToken);
    }
}