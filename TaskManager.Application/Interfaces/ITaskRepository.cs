using TaskEntity = TaskManager.Domain.Entities.Task;
using System.Threading.Tasks;

namespace TaskManager.Application.Interfaces;

public interface ITaskRepository
{
    Task<TaskEntity> CreateAsync(TaskEntity task, CancellationToken cancellationToken = default);
    Task<TaskEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<(IEnumerable<TaskEntity> Tasks, int TotalCount)> GetAllAsync(
        string? status,
        string? priority,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default);
    Task<TaskEntity> UpdateAsync(TaskEntity task, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}