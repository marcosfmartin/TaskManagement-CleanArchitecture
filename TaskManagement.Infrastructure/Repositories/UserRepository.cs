using Microsoft.Data.Sqlite;
using TaskManagement.Domain.Entities;
using TaskManagement.Domain.Interfaces;

namespace TaskManagement.Infrastructure.Repositories;

public class TaskRepository : ITaskRepository
{
    private readonly string _connectionString;

    public TaskRepository(string connectionString)
    {
        _connectionString = connectionString;
    }

    public Task<TaskItem> AddAsync(TaskItem task)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<TaskItem>> GetByUserIdAsync(int userId)
    {
        throw new NotImplementedException();
    }

    public Task<TaskItem?> GetByIdAsync(int id, int userId)
    {
        throw new NotImplementedException();
    }

    public Task<bool> UpdateAsync(TaskItem task)
    {
        throw new NotImplementedException();
    }

    public Task<bool> DeleteAsync(int id, int userId)
    {
        throw new NotImplementedException();
    }
}