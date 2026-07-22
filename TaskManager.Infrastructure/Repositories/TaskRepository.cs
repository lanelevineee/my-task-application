using Dapper;
using Microsoft.Extensions.Configuration;
using Npgsql;
using TaskManager.Application.Interfaces;
using TaskEntity = TaskManager.Domain.Entities.Task;
using System.Threading.Tasks;

namespace TaskManager.Infrastructure.Repositories;

public class TaskRepository : ITaskRepository
{
    private readonly string _connectionString;

    public TaskRepository(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection") 
            ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found");
    }

    private NpgsqlConnection CreateConnection() => new NpgsqlConnection(_connectionString);

    public async Task<TaskEntity> CreateAsync(TaskEntity task, CancellationToken cancellationToken = default)
    {
        using var connection = CreateConnection();
        await connection.OpenAsync(cancellationToken);

        const string sql = @"
            INSERT INTO tasks (id, title, description, status, priority, due_date, created_at, updated_at)
            VALUES (@Id, @Title, @Description, @Status, @Priority, @DueDate, @CreatedAt, @UpdatedAt)
            RETURNING id, title, description, status, priority, due_date, created_at, updated_at";

        var created = await connection.QuerySingleAsync<TaskEntity>(sql, task);
        return created;
    }

    public async Task<TaskEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        using var connection = CreateConnection();
        await connection.OpenAsync(cancellationToken);

        const string sql = "SELECT id, title, description, status, priority, due_date, created_at, updated_at FROM tasks WHERE id = @Id";
        return await connection.QueryFirstOrDefaultAsync<TaskEntity>(sql, new { Id = id });
    }

    public async Task<(IEnumerable<TaskEntity> Tasks, int TotalCount)> GetAllAsync(
        string? status,
        string? priority,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        using var connection = CreateConnection();
        await connection.OpenAsync(cancellationToken);

        var whereClauses = new List<string>();
        var parameters = new DynamicParameters();

        if (!string.IsNullOrEmpty(status))
        {
            whereClauses.Add("status = @Status");
            parameters.Add("Status", status);
        }

        if (!string.IsNullOrEmpty(priority))
        {
            whereClauses.Add("priority = @Priority");
            parameters.Add("Priority", priority);
        }

        var whereClause = whereClauses.Count > 0 ? "WHERE " + string.Join(" AND ", whereClauses) : "";
        
        var countSql = $"SELECT COUNT(*) FROM tasks {whereClause}";
        var totalCount = await connection.ExecuteScalarAsync<int>(countSql, parameters);

        var offset = (page - 1) * pageSize;
        var selectSql = $@"
            SELECT id, title, description, status, priority, due_date, created_at, updated_at
            FROM tasks
            {whereClause}
            ORDER BY created_at DESC
            LIMIT @PageSize OFFSET @Offset";

        parameters.Add("PageSize", pageSize);
        parameters.Add("Offset", offset);

        var tasks = await connection.QueryAsync<TaskEntity>(selectSql, parameters);
        
        return (tasks, totalCount);
    }

    public async Task<TaskEntity> UpdateAsync(TaskEntity task, CancellationToken cancellationToken = default)
    {
        using var connection = CreateConnection();
        await connection.OpenAsync(cancellationToken);

        const string sql = @"
            UPDATE tasks
            SET title = @Title,
                description = @Description,
                status = @Status,
                priority = @Priority,
                due_date = @DueDate,
                updated_at = @UpdatedAt
            WHERE id = @Id
            RETURNING id, title, description, status, priority, due_date, created_at, updated_at";

        task.UpdatedAt = DateTime.UtcNow;
        var updated = await connection.QuerySingleAsync<TaskEntity>(sql, task);
        return updated;
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        using var connection = CreateConnection();
        await connection.OpenAsync(cancellationToken);

        const string sql = "DELETE FROM tasks WHERE id = @Id";
        var affected = await connection.ExecuteAsync(sql, new { Id = id });
        return affected > 0;
    }
}