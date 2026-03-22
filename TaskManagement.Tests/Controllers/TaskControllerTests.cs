using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Moq;
using TaskManagement.API.Controllers;
using TaskManagement.Application.Services;
using TaskManagement.Application.DTOs;
using System.Security.Claims;
using Xunit;

namespace TaskManagement.Tests.Controllers;

public class TaskControllerTests
{
    private readonly Mock<ITaskService> _mockTaskService;
    private readonly TasksController _controller;

    public TaskControllerTests()
    {
        _mockTaskService = new Mock<ITaskService>();
        _controller = new TasksController(_mockTaskService.Object);

        // Simulate a logged-in user with ID 1
        var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
        {
            new Claim(ClaimTypes.NameIdentifier, "1")
        }, "mock"));

        _controller.ControllerContext = new ControllerContext()
        {
            HttpContext = new DefaultHttpContext() { User = user }
        };
    }

    [Fact]
    public async Task CreateTask_ValidData_ReturnsCreated()
    {
        // Arrange
        var dto = new CreateTaskDto { Title = "Test Task" };
        var createdTask = new TaskManagement.Domain.Entities.TaskItem
        {
            Id = 1,
            Title = "Test Task",
            UserId = 1
        };

        _mockTaskService.Setup(s => s.CreateTaskAsync(1, dto))
                        .ReturnsAsync(createdTask);

        // Act - Call the actual method name in your controller
        var result = await _controller.CreateTask(dto);

        // Assert - Match the 'Created' return type in your controller
        var createdResult = Assert.IsType<CreatedResult>(result);
        Assert.Equal(201, createdResult.StatusCode);
        Assert.Equal($"/api/tasks/{createdTask.Id}", createdResult.Location);
    }

    [Fact]
    public async Task GetTask_TaskNotFound_ReturnsNotFound()
    {
        // Arrange
        _mockTaskService.Setup(s => s.GetTaskByIdAsync(99, 1))
                        .ReturnsAsync((TaskManagement.Domain.Entities.TaskItem?)null);

        // Act - Call the actual method name in your controller
        var result = await _controller.GetTask(99);

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }
}