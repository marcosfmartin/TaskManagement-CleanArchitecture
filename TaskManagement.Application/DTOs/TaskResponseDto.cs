namespace TaskManagement.Application.DTOs;

public class TaskResponseDto : TaskBaseDto
{
    public int Id { get; set; }

    public string Status { get; set; } = "Pending";

    public int UserId { get; set; }
}