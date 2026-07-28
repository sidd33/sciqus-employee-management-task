namespace EmployeeManagement.BUSINESS.BusinessModels.ResponseDTOs.TicketResponseDtos;

public class TicketResponseDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public Guid CustomerId { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public string CustomerEmail { get; set; } = string.Empty;
    public Guid? AssignedEmployeeId { get; set; }
    public Guid DepartmentId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime SlaStartTime { get; set; }
    public bool IsSlaBreached { get; set; }
}
