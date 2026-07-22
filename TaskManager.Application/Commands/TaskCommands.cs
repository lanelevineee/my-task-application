using MediatR;
using TaskManager.Application.DTOs;

namespace TaskManager.Application.Commands;

public record CreateTaskCommand(CreateTaskDto Task) : IRequest<TaskDto>;

public record UpdateTaskCommand(Guid Id, UpdateTaskDto Task) : IRequest<TaskDto>;

public record DeleteTaskCommand(Guid Id) : IRequest<bool>;