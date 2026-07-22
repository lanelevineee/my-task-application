using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using TaskManager.Application.Handlers.Queries;
using TaskManager.Application.Interfaces;
using TaskManager.Application.Queries;
using TaskManager.Domain.Entities;
using Task = TaskManager.Domain.Entities.Task;

namespace TaskManager.Tests.Handlers;

public class GetTaskByIdQueryHandlerTests
{
    private readonly Mock<ITaskRepository> _repositoryMock;
    private readonly GetTaskByIdQueryHandler _handler;

    public GetTaskByIdQueryHandlerTests()
    {
        _repositoryMock = new Mock<ITaskRepository>();
        _handler = new GetTaskByIdQueryHandler(_repositoryMock.Object);
    }

    [Fact]
    public async System.Threading.Tasks.Task Handle_WithExistingTask_ShouldReturnTaskDto()
    {
        // Arrange
        var taskId = Guid.NewGuid();
        var task = new Task
        {
            Id = taskId,
            Title = "Test Task",
            Description = "Test Description",
            Status = Domain.Enums.TaskStatus.Pending,
            Priority = Domain.Enums.TaskPriority.Medium,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        var query = new GetTaskByIdQuery(taskId);

        _repositoryMock.Setup(x => x.GetByIdAsync(taskId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(task);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(taskId);
        result.Title.Should().Be("Test Task");
        result.Status.Should().Be(Domain.Enums.TaskStatus.Pending);
    }

    [Fact]
    public async System.Threading.Tasks.Task Handle_WithNonExistentTask_ShouldReturnNull()
    {
        // Arrange
        var taskId = Guid.NewGuid();
        var query = new GetTaskByIdQuery(taskId);

        _repositoryMock.Setup(x => x.GetByIdAsync(taskId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Task?)null);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().BeNull();
    }
}