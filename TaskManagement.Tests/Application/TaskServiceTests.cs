using Moq;
using Xunit;
using TaskManagement.Application.Services;
using TaskManagement.Application.DTOs;
using TaskManagement.Domain.Entities;
using TaskManagement.Domain.Interfaces;

namespace TaskManagement.Tests;

public class TaskServiceTests
{
    private readonly Mock<ITaskRepository> _mockTaskRepo;
    private readonly TaskService _taskService;

    public TaskServiceTests()
    {
        _mockTaskRepo = new Mock<ITaskRepository>();
        _taskService = new TaskService(_mockTaskRepo.Object);
    }

    [Fact]
    public async Task CreateTaskAsync_WithValidData_ReturnsCreatedTask()
    {
        // Arrange
        var userId = 1;
        var dto = new CreateTaskDto
        {
            Title = "Interview Prep",
            Description = "Review Clean Architecture",
            DueDate = DateTime.UtcNow.AddDays(1)
        };

        var expectedTask = new TaskItem
        {
            Id = 1,
            UserId = userId,
            Title = dto.Title,
            Description = dto.Description,
            DueDate = dto.DueDate,
            Status = "Pending"
        };

        _mockTaskRepo.Setup(repo => repo.AddAsync(It.IsAny<TaskItem>()))
                     .ReturnsAsync(expectedTask);

        // Act
        var result = await _taskService.CreateTaskAsync(userId, dto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Interview Prep", result.Title);
        Assert.Equal(1, result.Id);
        _mockTaskRepo.Verify(repo => repo.AddAsync(It.IsAny<TaskItem>()), Times.Once);
    }

    [Fact]
    public async Task CreateTaskAsync_WithPastDueDate_ThrowsArgumentException()
    {
        // Arrange
        var userId = 1;
        var dto = new CreateTaskDto
        {
            Title = "Too Late",
            Description = "This is in the past",
            DueDate = DateTime.UtcNow.AddDays(-1)
        };

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ArgumentException>(() =>
            _taskService.CreateTaskAsync(userId, dto));

        Assert.Equal("Due date cannot be in the past.", exception.Message);
        _mockTaskRepo.Verify(repo => repo.AddAsync(It.IsAny<TaskItem>()), Times.Never);
    }

    [Fact]
    public async Task GetTasksByUserIdAsync_ReturnsUserSpecificTasks()
    {
        // Arrange
        var userId = 10;
        var tasks = new List<TaskItem> {
            new TaskItem { Id = 1, UserId = userId, Title = "Task 1" },
            new TaskItem { Id = 2, UserId = userId, Title = "Task 2" }
        };
        _mockTaskRepo.Setup(repo => repo.GetByUserIdAsync(userId)).ReturnsAsync(tasks);

        // Act
        var result = await _taskService.GetTasksByUserIdAsync(userId);

        // Assert
        Assert.Equal(2, result.Count());
        _mockTaskRepo.Verify(repo => repo.GetByUserIdAsync(userId), Times.Once);
    }

    [Fact]
    public async Task UpdateTaskAsync_ExistingTask_ReturnsTrue()
    {
        // Arrange
        var taskId = 1;
        var userId = 1;
        var existingTask = new TaskItem { Id = taskId, UserId = userId };
        var updateDto = new UpdateTaskDto { Title = "Updated", Status = "Completed" };

        _mockTaskRepo.Setup(repo => repo.GetByIdAsync(taskId, userId)).ReturnsAsync(existingTask);
        _mockTaskRepo.Setup(repo => repo.UpdateAsync(It.IsAny<TaskItem>())).ReturnsAsync(true);

        // Act
        var result = await _taskService.UpdateTaskAsync(taskId, userId, updateDto);

        // Assert
        Assert.True(result);
        _mockTaskRepo.Verify(repo => repo.UpdateAsync(It.IsAny<TaskItem>()), Times.Once);
    }

    [Fact]
    public async Task UpdateTaskAsync_NonExistentTask_ReturnsFalse()
    {
        // Arrange
        _mockTaskRepo.Setup(repo => repo.GetByIdAsync(It.IsAny<int>(), It.IsAny<int>()))
                     .ReturnsAsync((TaskItem?)null);

        // Act
        var result = await _taskService.UpdateTaskAsync(99, 1, new UpdateTaskDto());

        // Assert
        Assert.False(result);
        _mockTaskRepo.Verify(repo => repo.UpdateAsync(It.IsAny<TaskItem>()), Times.Never);
    }

    [Fact]
    public async Task DeleteTaskAsync_CallsRepoWithCorrectIds()
    {
        // Arrange
        var taskId = 5;
        var userId = 1;
        _mockTaskRepo.Setup(repo => repo.DeleteAsync(taskId, userId)).ReturnsAsync(true);

        // Act
        var result = await _taskService.DeleteTaskAsync(taskId, userId);

        // Assert
        Assert.True(result);
        _mockTaskRepo.Verify(repo => repo.DeleteAsync(taskId, userId), Times.Once);
    }
}