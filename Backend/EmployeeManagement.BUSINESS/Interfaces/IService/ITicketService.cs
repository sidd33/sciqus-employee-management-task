namespace EmployeeManagement.BUSINESS.Interfaces.IService;

using EmployeeManagement.DATA.DomainModels.TicketDATA;

public interface ITicketService
{
    Task<Ticket> CreateTicketAsync(Ticket ticket);
    Task<Ticket?> GetTicketByIdAsync(Guid id);
    Task<IEnumerable<Ticket>> GetAllTicketsAsync();
    Task<Ticket?> UpdateTicketAsync(Ticket ticket);
}
