namespace TaskManagement.Application.DTOs;

public abstract class TaskBaseDto
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime DueDate { get; set; }
}