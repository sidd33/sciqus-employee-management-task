namespace EmployeeManagement.BUSINESS.BusinessModels.RequestDTOs.TicketRequestDtos;

using EmployeeManagement.DATA.DomainModels.TicketDATA;

public class TicketQueryParameters
{
    public TicketStatus? Status { get; set; }
    public Guid? DepartmentId { get; set; }
    public Guid? CustomerId { get; set; }
    public string? SearchTerm { get; set; }

    public string SortBy { get; set; } = "CreatedAt";
    public bool IsDescending { get; set; } = true;

    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}
