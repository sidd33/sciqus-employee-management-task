namespace EmployeeManagement.BUSINESS.BusinessModels.RequestDTOs.TicketRequestDtos;

using EmployeeManagement.DATA.DomainModels.TicketDATA;

public class UpdateTicketDto
{
    public string? Title { get; set; }
    public string? Description { get; set; }
    public TicketStatus? Status { get; set; }
    public Guid? DepartmentId { get; set; }
}
