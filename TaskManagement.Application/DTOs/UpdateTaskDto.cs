namespace TaskManagement.Application.DTOs;

public class UpdateTaskDto : TaskBaseDto
{
    public string Status { get; set; } = "Pending";
}