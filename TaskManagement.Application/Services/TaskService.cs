using TaskManagement.Application.DTOs;
using TaskManagement.Domain.Entities;
using TaskManagement.Domain.Interfaces;

namespace TaskManagement.Application.Services;

public interface ITaskService
{
    Task<TaskItem> CreateTaskAsync(int userId, CreateTaskDto dto);
    Task<IEnumerable<TaskItem>> GetTasksByUserIdAsync(int userId);
    Task<TaskItem?> GetTaskByIdAsync(int id, int userId);
    Task<bool> UpdateTaskAsync(int id, int userId, UpdateTaskDto dto);
    Task<bool> DeleteTaskAsync(int id, int userId);
}

public class TaskService : ITaskService
{
    private readonly ITaskRepository _taskRepository;

    public TaskService(ITaskRepository taskRepository)
    {
        _taskRepository = taskRepository;
    }

    public async Task<TaskItem> CreateTaskAsync(int userId, CreateTaskDto dto)
    {
        if (dto.DueDate < DateTime.UtcNow.Date)
        {
            throw new ArgumentException("Due date cannot be in the past.");
        }

        var taskItem = new TaskItem
        {
            UserId = userId,
            Title = dto.Title,
            Description = dto.Description,
            DueDate = dto.DueDate,
            Status = "Pending"
        };

        return await _taskRepository.AddAsync(taskItem);
    }

    public Task<IEnumerable<TaskItem>> GetTasksByUserIdAsync(int userId)
    {
        throw new NotImplementedException();
    }

    public Task<TaskItem?> GetTaskByIdAsync(int id, int userId)
    {
        throw new NotImplementedException();
    }

    public Task<bool> UpdateTaskAsync(int id, int userId, UpdateTaskDto dto)
    {
        throw new NotImplementedException();
    }

    public Task<bool> DeleteTaskAsync(int id, int userId)
    {
        throw new NotImplementedException();
    }
}