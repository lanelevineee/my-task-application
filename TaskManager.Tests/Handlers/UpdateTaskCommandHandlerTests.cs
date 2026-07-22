using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using TaskManager.Application.Commands;
using TaskManager.Application.DTOs;
using TaskManager.Application.Handlers.Commands;
using TaskManager.Application.Interfaces;
using TaskManager.Domain.Entities;
using Task = TaskManager.Domain.Entities.Task;

namespace TaskManager.Tests.Handlers;

public class UpdateTaskCommandHandlerTests
{
    private readonly Mock<ITaskRepository> _repositoryMock;
    private readonly UpdateTaskCommandHandler _handler;

    public UpdateTaskCommandHandlerTests()
    {
        _repositoryMock = new Mock<ITaskRepository>();
        _handler = new UpdateTaskCommandHandler(_repositoryMock.Object);
    }

    [Fact]
    public async System.Threading.Tasks.Task Handle_WithExistingTask_ShouldUpdateTask()
    {
        // Arrange
        var taskId = Guid.NewGuid();
        var existingTask = new Task
        {
            Id = taskId,
            Title = "Old Title",
            Description = "Old Description",
            Status = Domain.Enums.TaskStatus.Pending,
            Priority = Domain.Enums.TaskPriority.Low,
            CreatedAt = DateTime.UtcNow.AddDays(-1),
            UpdatedAt = DateTime.UtcNow.AddDays(-1)
        };

        var updateDto = new UpdateTaskDto("New Title", "New Description", Domain.Enums.TaskStatus.Completed, Domain.Enums.TaskPriority.High, null);
        var command = new UpdateTaskCommand(taskId, updateDto);

        _repositoryMock.Setup(x => x.GetByIdAsync(taskId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingTask);

        _repositoryMock.Setup(x => x.UpdateAsync(It.IsAny<Task>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Task task, CancellationToken ct) => task);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Title.Should().Be("New Title");
        result.Description.Should().Be("New Description");
        result.Status.Should().Be(Domain.Enums.TaskStatus.Completed);
        result.Priority.Should().Be(Domain.Enums.TaskPriority.High);
        _repositoryMock.Verify(x => x.UpdateAsync(It.IsAny<Task>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async System.Threading.Tasks.Task Handle_WithNonExistentTask_ShouldThrowKeyNotFoundException()
    {
        // Arrange
        var taskId = Guid.NewGuid();
        var updateDto = new UpdateTaskDto("New Title", null, null, null, null);
        var command = new UpdateTaskCommand(taskId, updateDto);

        _repositoryMock.Setup(x => x.GetByIdAsync(taskId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Task?)null);

        // Act & Assert
        await _handler.Invoking(h => h.Handle(command, CancellationToken.None))
            .Should().ThrowAsync<KeyNotFoundException>()
            .WithMessage($"*{taskId}*");
    }

    [Fact]
    public async System.Threading.Tasks.Task Handle_WithNullOptionalFields_ShouldPreserveExistingStatusAndPriority()
    {
        // Arrange
        var taskId = Guid.NewGuid();
        var existingTask = new Task
        {
            Id = taskId,
            Title = "Original Title",
            Description = "Original Description",
            Status = Domain.Enums.TaskStatus.Pending,
            Priority = Domain.Enums.TaskPriority.Low,
            CreatedAt = DateTime.UtcNow.AddDays(-1),
            UpdatedAt = DateTime.UtcNow.AddDays(-1)
        };

        var updateDto = new UpdateTaskDto("Updated Title", "Updated Description", null, null, null);
        var command = new UpdateTaskCommand(taskId, updateDto);

        _repositoryMock.Setup(x => x.GetByIdAsync(taskId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingTask);

        _repositoryMock.Setup(x => x.UpdateAsync(It.IsAny<Task>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Task task, CancellationToken ct) => task);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Title.Should().Be("Updated Title");
        result.Description.Should().Be("Updated Description");
        result.Status.Should().Be(Domain.Enums.TaskStatus.Pending);
        result.Priority.Should().Be(Domain.Enums.TaskPriority.Low);
    }
}