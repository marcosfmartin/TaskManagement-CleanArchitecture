using TaskManagement.Domain.Entities;
    
namespace TaskManagement.Domain.Interfaces;
public interface ITaskRepository
{
    Task<TaskItem> AddAsync(TaskItem task);
    Task<IEnumerable<TaskItem>> GetByUserIdAsync(int userId);
    Task<TaskItem?> GetByIdAsync(int id, int userId);
    Task<bool> UpdateAsync(TaskItem task);
    Task<bool> DeleteAsync(int id, int userId);
}