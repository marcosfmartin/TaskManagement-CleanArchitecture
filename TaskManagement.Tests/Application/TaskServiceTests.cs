using Moq;
using Xunit;

public class TaskServiceTests
{
    private readonly Mock<ITaskRepository> _mockTaskRepo;
    private readonly TaskService _taskService;

    public TaskServiceTests()
    {
        // Arrange: Setup the mock repository
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
            DueDate = DateTime.UtcNow.AddDays(-1) // Past date!
        };

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ArgumentException>(() =>
            _taskService.CreateTaskAsync(userId, dto));

        Assert.Equal("Due date cannot be in the past.", exception.Message);

        // Ensure the repository was never called
        _mockTaskRepo.Verify(repo => repo.AddAsync(It.IsAny<TaskItem>()), Times.Never);
    }
}