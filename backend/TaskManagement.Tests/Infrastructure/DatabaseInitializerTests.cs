using Microsoft.Data.Sqlite;
using TaskManagement.Infrastructure.Data;
using Xunit;

namespace TaskManagement.Tests.Infrastructure;

public class DatabaseInitializerTests : IDisposable
{
    private readonly string _connectionString;
    private readonly SqliteConnection _masterConnection;

    public DatabaseInitializerTests()
    {
        // Use a unique name to avoid interference with Repo tests
        _connectionString = "Data Source=InitTest;Mode=Memory;Cache=Shared";
        _masterConnection = new SqliteConnection(_connectionString);
        _masterConnection.Open();
    }

    [Fact]
    public void Initialize_ShouldCreateTablesWithCorrectSchema()
    {
        // Arrange
        var initializer = new DatabaseInitializer(_connectionString);

        // Act
        initializer.Initialize();

        // Assert - Check if Tables exist
        Assert.True(TableExists("Users"));
        Assert.True(TableExists("Tasks"));

        // Assert - Check specific columns to ensure SQL string was correct
        Assert.True(ColumnExists("Tasks", "Status"));
        Assert.True(ColumnExists("Tasks", "UserId"));
    }

    private bool TableExists(string tableName)
    {
        using var cmd = new SqliteCommand(
            "SELECT name FROM sqlite_master WHERE type='table' AND name=@name",
            _masterConnection);
        cmd.Parameters.AddWithValue("@name", tableName);
        return cmd.ExecuteScalar() != null;
    }

    private bool ColumnExists(string tableName, string columnName)
    {
        using var cmd = new SqliteCommand($"PRAGMA table_info({tableName})", _masterConnection);
        using var reader = cmd.ExecuteReader();
        while (reader.Read())
        {
            if (reader.GetString(1).Equals(columnName, StringComparison.OrdinalIgnoreCase))
                return true;
        }
        return false;
    }

    public void Dispose()
    {
        _masterConnection.Close();
        _masterConnection.Dispose();
    }
}