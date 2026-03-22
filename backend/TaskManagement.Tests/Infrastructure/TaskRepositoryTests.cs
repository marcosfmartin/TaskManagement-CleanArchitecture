using Microsoft.Data.Sqlite;
using TaskManagement.Domain.Entities;
using TaskManagement.Domain.Interfaces; // Use the interface namespace
using TaskManagement.Infrastructure.Repositories;
using Xunit;

namespace TaskManagement.Tests.Infrastructure;

public class TaskRepositoryTests : IDisposable
{
    private readonly SqliteConnection _masterConnection;
    private readonly string _connectionString;

    // 1. Change to the Interface
    private readonly ITaskRepository _repository;

    public TaskRepositoryTests()
    {
        _connectionString = "Data Source=TaskRepoTest;Mode=Memory;Cache=Shared";
        _masterConnection = new SqliteConnection(_connectionString);
        _masterConnection.Open();

        InitializeDatabase();

        // 2. Instantiate the concrete class, but store it as the interface
        _repository = new TaskRepository(_connectionString);
    }

    private void InitializeDatabase()
    {
        var createTableSql = @"
            CREATE TABLE Tasks (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                UserId INTEGER NOT NULL,
                Title TEXT NOT NULL,
                Description TEXT,
                Status TEXT NOT NULL,
                DueDate TEXT NOT NULL
            );";

        using var command = new SqliteCommand(createTableSql, _masterConnection);
        command.ExecuteNonQuery();
    }

    [Fact]
    public async Task AddAsync_SuccessfullyInsertsAndReturnsId()
    {
        var task = new TaskItem
        {
            UserId = 1,
            Title = "Test Task",
            Status = "Pending",
            DueDate = DateTime.UtcNow.AddDays(1)
        };

        // This will call the NotImplementedException in your Red State implementation
        var result = await _repository.AddAsync(task);

        Assert.NotEqual(0, result.Id);
    }

    [Fact]
    public async Task GetByUserIdAsync_ReturnsOnlySpecificUserTasks()
    {
        using (var cmd = new SqliteCommand("INSERT INTO Tasks (UserId, Title, Status, DueDate) VALUES (1, 'T1', 'P', '2026-01-01')", _masterConnection))
        {
            await cmd.ExecuteNonQueryAsync();
        }

        var results = await _repository.GetByUserIdAsync(1);

        Assert.Single(results);
    }

    // ... other tests remain the same

    public void Dispose()
    {
        _masterConnection.Close();
        _masterConnection.Dispose();
    }
}