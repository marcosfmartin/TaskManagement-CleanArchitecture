using Microsoft.Data.Sqlite;
using TaskManagement.Domain.Entities;
using TaskManagement.Domain.Interfaces;
using TaskManagement.Infrastructure.Repositories;
using Xunit;

namespace TaskManagement.Tests.Infrastructure;

public class UserRepositoryTests : IDisposable
{
    private readonly SqliteConnection _masterConnection;
    private readonly string _connectionString;
    private readonly UserRepository _repository;

    public UserRepositoryTests()
    {
        // Use a unique name for the UserRepo test memory space
        _connectionString = "Data Source=UserRepoTest;Mode=Memory;Cache=Shared";
        _masterConnection = new SqliteConnection(_connectionString);
        _masterConnection.Open();

        InitializeDatabase();

        _repository = new UserRepository(_connectionString);
    }

    private void InitializeDatabase()
    {
        var createTableSql = @"
            CREATE TABLE Users (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                Username TEXT NOT NULL UNIQUE,
                PasswordHash TEXT NOT NULL
            );";

        using var command = new SqliteCommand(createTableSql, _masterConnection);
        command.ExecuteNonQuery();
    }

    [Fact]
    public async Task CreateAsync_ShouldSaveUserAndReturnId()
    {
        // Arrange
        var user = new User
        {
            Username = "tester",
            PasswordHash = "hashed_pw_123"
        };

        // Act
        var result = await _repository.CreateAsync(user);

        // Assert
        Assert.NotEqual(0, result.Id);
        Assert.Equal("tester", result.Username);

        // Manual verify against master connection
        using var checkCmd = new SqliteCommand("SELECT Username FROM Users WHERE Id = @id", _masterConnection);
        checkCmd.Parameters.AddWithValue("@id", result.Id);
        var dbUsername = (string?)checkCmd.ExecuteScalar();
        Assert.Equal("tester", dbUsername);
    }

    [Fact]
    public async Task GetByUsernameAsync_WhenUserExists_ReturnsUser()
    {
        // Arrange
        var username = "findme";
        using (var cmd = new SqliteCommand("INSERT INTO Users (Username, PasswordHash) VALUES (@u, @p)", _masterConnection))
        {
            cmd.Parameters.AddWithValue("@u", username);
            cmd.Parameters.AddWithValue("@p", "some_hash");
            await cmd.ExecuteNonQueryAsync();
        }

        // Act
        var result = await _repository.GetByUsernameAsync(username);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(username, result?.Username);
    }

    [Fact]
    public async Task GetByUsernameAsync_WhenUserDoesNotExist_ReturnsNull()
    {
        // Act
        var result = await _repository.GetByUsernameAsync("nonexistent");

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsCorrectUser()
    {
        // Arrange
        using (var cmd = new SqliteCommand("INSERT INTO Users (Username, PasswordHash) VALUES ('idtest', 'hash')", _masterConnection))
        {
            await cmd.ExecuteNonQueryAsync();
        }
        var expectedId = 1; // First insert in fresh DB

        // Act
        var result = await _repository.GetByIdAsync(expectedId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("idtest", result?.Username);
    }

    public void Dispose()
    {
        _masterConnection.Close();
        _masterConnection.Dispose();
    }
}