using Microsoft.Data.Sqlite;

namespace TaskManagement.Infrastructure.Data;

public class DatabaseInitializer
{
    private readonly string _connectionString;

    public DatabaseInitializer(string connectionString)
    {
        _connectionString = connectionString;
    }

    public void Initialize()
    {
        using var connection = new SqliteConnection(_connectionString);
        connection.Open();

        var command = connection.CreateCommand();
        command.CommandText = @"
            CREATE TABLE IF NOT EXISTS Users (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                Username TEXT NOT NULL UNIQUE,
                PasswordHash TEXT NOT NULL
            );

            CREATE TABLE IF NOT EXISTS Tasks (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                UserId INTEGER NOT NULL,
                Title TEXT NOT NULL,
                Description TEXT,
                Status TEXT DEFAULT 'Pending',
                DueDate TEXT NOT NULL,
                FOREIGN KEY(UserId) REFERENCES Users(Id)
            );

            -- ==========================================
            -- SEED DATA
            -- ==========================================
            
            -- WARNING: Ensure this hash matches 'password123' based on your Auth implementation!
            INSERT OR IGNORE INTO Users (Id, Username, PasswordHash) 
            VALUES (1, 'demouser', '$2a$11$XaQOWvhTsiketwwqyusdd.6js6bcLmN6bmiXB.shl1dS5Ga5MRF0m');

            -- Insert Demo Tasks for the Demo User
            INSERT OR IGNORE INTO Tasks (Id, UserId, Title, Description, Status, DueDate) 
            VALUES 
            (1, 1, 'Technical Interview Presentation', 'Present the task management app to the panel', 'Pending', '2024-12-31'),
            (2, 1, 'Finish Unit Tests', 'Write xUnit tests for the repository layer', 'Completed', '2024-10-15'),
            (3, 1, 'Review Clean Architecture', 'Ensure no EF Core is used and layers are separated', 'Pending', '2024-11-01');
        ";

        command.ExecuteNonQuery();
    }
}