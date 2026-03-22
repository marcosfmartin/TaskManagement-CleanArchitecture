using Microsoft.Data.Sqlite;
using TaskManagement.Domain.Entities;
using TaskManagement.Domain.Interfaces;

namespace TaskManagement.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly string _connectionString;

    public UserRepository(string connectionString)
    {
        _connectionString = connectionString;
    }

    public async Task<User?> GetByUsernameAsync(string username)
    {
        using var connection = new SqliteConnection(_connectionString);
        await connection.OpenAsync();

        var command = connection.CreateCommand();
        command.CommandText = "SELECT Id, Username, PasswordHash FROM Users WHERE Username = @Username";
        command.Parameters.AddWithValue("@Username", username);

        using var reader = await command.ExecuteReaderAsync();
        if (await reader.ReadAsync())
        {
            return new User
            {
                Id = reader.GetInt32(0),
                Username = reader.GetString(1),
                PasswordHash = reader.GetString(2)
            };
        }

        return null;
    }

    public async Task<User> CreateAsync(User user)
    {
        using var connection = new SqliteConnection(_connectionString);
        await connection.OpenAsync();

        var command = connection.CreateCommand();
        command.CommandText = @"
            INSERT INTO Users (Username, PasswordHash)
            VALUES (@Username, @PasswordHash);
            SELECT last_insert_rowid();
        ";

        command.Parameters.AddWithValue("@Username", user.Username);
        command.Parameters.AddWithValue("@PasswordHash", user.PasswordHash);

        // Safe unboxing of the new ID
        var result = await command.ExecuteScalarAsync();
        user.Id = (int)(long)(result ?? 0L);

        return user;
    }

    public async Task<User?> GetByIdAsync(int id)
    {
        using var connection = new SqliteConnection(_connectionString);
        await connection.OpenAsync();

        var command = connection.CreateCommand();
        command.CommandText = "SELECT Id, Username, PasswordHash FROM Users WHERE Id = @Id";
        command.Parameters.AddWithValue("@Id", id);

        using var reader = await command.ExecuteReaderAsync();
        if (await reader.ReadAsync())
        {
            return new User
            {
                Id = reader.GetInt32(0),
                Username = reader.GetString(1),
                PasswordHash = reader.GetString(2)
            };
        }

        return null;
    }
}