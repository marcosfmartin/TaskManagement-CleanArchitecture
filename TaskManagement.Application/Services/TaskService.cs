using TaskManagement.Application.DTOs;
using TaskManagement.Domain.Entities;
using TaskManagement.Domain.Interfaces;

namespace TaskManagement.Application.Services;

public interface ITaskService
{
    Task<TaskItem> CreateTaskAsync(int userId, CreateTaskDto dto);
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
}