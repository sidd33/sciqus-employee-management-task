namespace EmployeeManagement.BUSINESS.BusinessModels.RequestDTOs.TicketRequestDtos;

public class CreateTicketDto
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public Guid CustomerId { get; set; }
    public Guid DepartmentId { get; set; }
}
