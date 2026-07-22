using System.Threading.Tasks;
using TaskManager.Application.Interfaces;
using TaskManager.Domain.Entities;
using Task = TaskManager.Domain.Entities.Task;

namespace TaskManager.Tests.MockRepositories;

public class MockTaskRepository : ITaskRepository
{
    private readonly List<Task> _tasks = new();

    public MockTaskRepository()
    {
        _tasks.AddRange(new[]
        {
            new Task
            {
                Id = Guid.NewGuid(),
                Title = "Test Task 1",
                Description = "Description 1",
                Status = Domain.Enums.TaskStatus.Pending,
                Priority = Domain.Enums.TaskPriority.Medium,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new Task
            {
                Id = Guid.NewGuid(),
                Title = "Test Task 2",
                Description = "Description 2",
                Status = Domain.Enums.TaskStatus.InProgress,
                Priority = Domain.Enums.TaskPriority.High,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            }
        });
    }

    public System.Threading.Tasks.Task<Task> CreateAsync(Task task, CancellationToken cancellationToken = default)
    {
        task.Id = Guid.NewGuid();
        task.CreatedAt = DateTime.UtcNow;
        task.UpdatedAt = DateTime.UtcNow;
        _tasks.Add(task);
        return System.Threading.Tasks.Task.FromResult(task);
    }

    public System.Threading.Tasks.Task<Task?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var task = _tasks.FirstOrDefault(t => t.Id == id);
        return System.Threading.Tasks.Task.FromResult(task);
    }

    public System.Threading.Tasks.Task<(IEnumerable<Task> Tasks, int TotalCount)> GetAllAsync(
        Domain.Enums.TaskStatus? status,
        Domain.Enums.TaskPriority? priority,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        var query = _tasks.AsEnumerable();

        if (status.HasValue)
            query = query.Where(t => t.Status == status.Value);

        if (priority.HasValue)
            query = query.Where(t => t.Priority == priority.Value);

        var totalCount = query.Count();
        var tasks = query.Skip((page - 1) * pageSize).Take(pageSize).ToList();

        return System.Threading.Tasks.Task.FromResult((tasks.AsEnumerable(), totalCount));
    }

    public System.Threading.Tasks.Task<Task> UpdateAsync(Task task, CancellationToken cancellationToken = default)
    {
        var existing = _tasks.FirstOrDefault(t => t.Id == task.Id);
        if (existing != null)
        {
            existing.Title = task.Title;
            existing.Description = task.Description;
            existing.Status = task.Status;
            existing.Priority = task.Priority;
            existing.DueDate = task.DueDate;
            existing.UpdatedAt = DateTime.UtcNow;
        }
        return System.Threading.Tasks.Task.FromResult(existing ?? task);
    }

    public System.Threading.Tasks.Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var task = _tasks.FirstOrDefault(t => t.Id == id);
        if (task != null)
        {
            _tasks.Remove(task);
            return System.Threading.Tasks.Task.FromResult(true);
        }
        return System.Threading.Tasks.Task.FromResult(false);
    }
}