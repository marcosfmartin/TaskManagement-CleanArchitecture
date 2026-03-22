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

    public async Task<TaskItem> AddAsync(TaskItem task)
    {
        using var connection = new SqliteConnection(_connectionString);
        await connection.OpenAsync();

        var command = connection.CreateCommand();
        command.CommandText = @"
            INSERT INTO Tasks (UserId, Title, Description, Status, DueDate)
            VALUES (@UserId, @Title, @Description, @Status, @DueDate);
            SELECT last_insert_rowid();
        ";

        command.Parameters.AddWithValue("@UserId", task.UserId);
        command.Parameters.AddWithValue("@Title", task.Title);
        command.Parameters.AddWithValue("@Description", task.Description ?? (object)DBNull.Value);
        command.Parameters.AddWithValue("@Status", task.Status);
        command.Parameters.AddWithValue("@DueDate", task.DueDate.ToString("O"));

        var result = await command.ExecuteScalarAsync();
        if (result != null && result != DBNull.Value)
        {
            task.Id = (int)(long)result;
        }

        return task;
    }

    public async Task<IEnumerable<TaskItem>> GetByUserIdAsync(int userId)
    {
        var tasks = new List<TaskItem>();
        using var connection = new SqliteConnection(_connectionString);
        await connection.OpenAsync();

        var command = connection.CreateCommand();
        command.CommandText = "SELECT Id, UserId, Title, Description, Status, DueDate FROM Tasks WHERE UserId = @UserId";
        command.Parameters.AddWithValue("@UserId", userId);

        using var reader = await command.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            tasks.Add(MapReaderToTask(reader));
        }

        return tasks;
    }

    public async Task<TaskItem?> GetByIdAsync(int id, int userId)
    {
        using var connection = new SqliteConnection(_connectionString);
        await connection.OpenAsync();
        var command = connection.CreateCommand();
        command.CommandText = "SELECT Id, UserId, Title, Description, Status, DueDate FROM Tasks WHERE Id = @Id AND UserId = @UserId";
        command.Parameters.AddWithValue("@Id", id);
        command.Parameters.AddWithValue("@UserId", userId);

        using var reader = await command.ExecuteReaderAsync();
        if (await reader.ReadAsync())
        {
            return MapReaderToTask(reader);
        }
        return null;
    }

    public async Task<bool> UpdateAsync(TaskItem task)
    {
        using var connection = new SqliteConnection(_connectionString);
        await connection.OpenAsync();
        var command = connection.CreateCommand();
        command.CommandText = @"
            UPDATE Tasks 
            SET Title = @Title, Description = @Description, Status = @Status, DueDate = @DueDate
            WHERE Id = @Id AND UserId = @UserId";

        command.Parameters.AddWithValue("@Id", task.Id);
        command.Parameters.AddWithValue("@UserId", task.UserId);
        command.Parameters.AddWithValue("@Title", task.Title);
        command.Parameters.AddWithValue("@Description", task.Description ?? (object)DBNull.Value);
        command.Parameters.AddWithValue("@Status", task.Status);
        command.Parameters.AddWithValue("@DueDate", task.DueDate.ToString("O"));

        var rowsAffected = await command.ExecuteNonQueryAsync();
        return rowsAffected > 0;
    }

    public async Task<bool> DeleteAsync(int id, int userId)
    {
        using var connection = new SqliteConnection(_connectionString);
        await connection.OpenAsync();
        var command = connection.CreateCommand();
        command.CommandText = "DELETE FROM Tasks WHERE Id = @Id AND UserId = @UserId";
        command.Parameters.AddWithValue("@Id", id);
        command.Parameters.AddWithValue("@UserId", userId);

        var rowsAffected = await command.ExecuteNonQueryAsync();
        return rowsAffected > 0;
    }

    private TaskItem MapReaderToTask(SqliteDataReader reader)
    {
        return new TaskItem
        {
            Id = reader.GetInt32(0),
            UserId = reader.GetInt32(1),
            Title = reader.GetString(2),
            Description = reader.IsDBNull(3) ? string.Empty : reader.GetString(3),
            Status = reader.GetString(4),
            DueDate = DateTime.Parse(reader.GetString(5))
        };
    }
}