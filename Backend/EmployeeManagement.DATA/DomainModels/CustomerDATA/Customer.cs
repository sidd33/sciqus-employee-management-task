namespace EmployeeManagement.DATA.DomainModels.CustomerDATA;

using EmployeeManagement.DATA.DomainModels.TicketDATA;

public class Customer
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string? ProfilePicture { get; set; }

    public ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();
}
