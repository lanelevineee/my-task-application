using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using TaskManager.Application.Handlers.Queries;
using TaskManager.Application.Interfaces;
using TaskManager.Application.Queries;
using TaskManager.Domain.Entities;
using Task = TaskManager.Domain.Entities.Task;

namespace TaskManager.Tests.Handlers;

public class GetAllTasksQueryHandlerTests
{
    private readonly Mock<ITaskRepository> _repositoryMock;
    private readonly GetAllTasksQueryHandler _handler;

    public GetAllTasksQueryHandlerTests()
    {
        _repositoryMock = new Mock<ITaskRepository>();
        _handler = new GetAllTasksQueryHandler(_repositoryMock.Object);
    }

    [Fact]
    public async System.Threading.Tasks.Task Handle_WithTasks_ShouldReturnPagedResult()
    {
        // Arrange
        var tasks = new List<Task>
        {
            new() { Id = Guid.NewGuid(), Title = "Task 1", Status = Domain.Enums.TaskStatus.Pending, Priority = Domain.Enums.TaskPriority.Low },
            new() { Id = Guid.NewGuid(), Title = "Task 2", Status = Domain.Enums.TaskStatus.Completed, Priority = Domain.Enums.TaskPriority.High }
        };

        var query = new GetAllTasksQuery(Page: 1, PageSize: 10);

        _repositoryMock.Setup(x => x.GetAllAsync(
                It.IsAny<Domain.Enums.TaskStatus?>(),
                It.IsAny<Domain.Enums.TaskPriority?>(),
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((tasks.AsEnumerable(), 2));

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Items.Should().HaveCount(2);
        result.TotalCount.Should().Be(2);
        result.Page.Should().Be(1);
        result.PageSize.Should().Be(10);
    }

    [Fact]
    public async System.Threading.Tasks.Task Handle_WithEmptyRepository_ShouldReturnEmptyResult()
    {
        // Arrange
        var query = new GetAllTasksQuery(Page: 1, PageSize: 10);

        _repositoryMock.Setup(x => x.GetAllAsync(
                It.IsAny<Domain.Enums.TaskStatus?>(),
                It.IsAny<Domain.Enums.TaskPriority?>(),
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((Enumerable.Empty<Task>().AsEnumerable(), 0));

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Items.Should().BeEmpty();
        result.TotalCount.Should().Be(0);
    }
}