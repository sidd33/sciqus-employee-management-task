namespace EmployeeManagement.DATA.DomainModels.TicketDATA;

using EmployeeManagement.DATA.DomainModels.CustomerDATA;

public class Ticket
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;

    public Guid CustomerId { get; set; }
    public Customer Customer { get; set; } = null!;

    public Guid DepartmentId { get; set; }
    public Guid AssignedEmployeeId { get; set; }

    public TicketStatus Status { get; set; } = TicketStatus.Unassigned;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    public DateTime SlaStartTime { get; set; }
    public bool IsSlaBreached { get; set; } = false;
}
