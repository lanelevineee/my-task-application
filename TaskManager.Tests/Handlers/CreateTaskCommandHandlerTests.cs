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

public class CreateTaskCommandHandlerTests
{
    private readonly Mock<ITaskRepository> _repositoryMock;
    private readonly CreateTaskCommandHandler _handler;

    public CreateTaskCommandHandlerTests()
    {
        _repositoryMock = new Mock<ITaskRepository>();
        _handler = new CreateTaskCommandHandler(_repositoryMock.Object);
    }

    [Fact]
    public async System.Threading.Tasks.Task Handle_WithValidRequest_ShouldCreateTask()
    {
        // Arrange
        var dto = new CreateTaskDto("New Task", "Description", Domain.Enums.TaskStatus.Pending, Domain.Enums.TaskPriority.Medium);
        var command = new CreateTaskCommand(dto);

        _repositoryMock.Setup(x => x.CreateAsync(It.IsAny<Task>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Task task, CancellationToken ct) =>
            {
                task.Id = Guid.NewGuid();
                return task;
            });

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Title.Should().Be("New Task");
        result.Description.Should().Be("Description");
        result.Status.Should().Be(Domain.Enums.TaskStatus.Pending);
        result.Priority.Should().Be(Domain.Enums.TaskPriority.Medium);
        _repositoryMock.Verify(x => x.CreateAsync(It.IsAny<Task>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async System.Threading.Tasks.Task Handle_WithValidRequest_ShouldAssignNewId()
    {
        // Arrange
        var dto = new CreateTaskDto("New Task", null);
        var command = new CreateTaskCommand(dto);

        _repositoryMock.Setup(x => x.CreateAsync(It.IsAny<Task>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Task task, CancellationToken ct) => task);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Id.Should().NotBeEmpty();
    }

    [Fact]
    public async System.Threading.Tasks.Task Handle_WithValidRequest_ShouldSetTimestamps()
    {
        // Arrange
        var dto = new CreateTaskDto("New Task", null);
        var command = new CreateTaskCommand(dto);
        var beforeTest = DateTime.UtcNow;

        _repositoryMock.Setup(x => x.CreateAsync(It.IsAny<Task>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Task task, CancellationToken ct) => task);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.CreatedAt.Should().BeOnOrAfter(beforeTest);
        result.UpdatedAt.Should().BeOnOrAfter(beforeTest);
    }
}