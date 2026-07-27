namespace EmployeeManagement.BUSINESS.Implementations.Service;

using Microsoft.EntityFrameworkCore;
using EmployeeManagement.BUSINESS.BusinessModels.RequestDTOs.TicketRequestDtos;
using EmployeeManagement.BUSINESS.BusinessModels.ResponseDTOs.TicketResponseDtos;
using EmployeeManagement.BUSINESS.Interfaces.IService;
using EmployeeManagement.DATA.Contexts;
using EmployeeManagement.DATA.DomainModels.TicketDATA;

public class TicketService : ITicketService
{
    private readonly AppDbContext _context;

    public TicketService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Ticket> CreateTicketAsync(CreateTicketDto dto)
    {
        var ticket = new Ticket
        {
            Title = dto.Title,
            Description = dto.Description,
            CustomerId = dto.CustomerId,
            DepartmentId = dto.DepartmentId,
            Status = TicketStatus.Unassigned,
            AssignedEmployeeId = null,
            SlaStartTime = DateTime.UtcNow.AddHours(4),
            CreatedAt = DateTime.UtcNow
        };

        _context.Tickets.Add(ticket);
        await _context.SaveChangesAsync();
        return ticket;
    }

    public async Task<TicketResponseDto?> GetTicketByIdAsync(Guid id)
    {
        var ticket = await _context.Tickets
            .Include(t => t.Customer)
            .FirstOrDefaultAsync(t => t.Id == id);

        if (ticket == null) return null;

        return new TicketResponseDto
        {
            Id = ticket.Id,
            Title = ticket.Title,
            Description = ticket.Description,
            Status = ticket.Status.ToString(),
            CustomerId = ticket.CustomerId,
            CustomerName = ticket.Customer?.Name ?? string.Empty,
            CustomerEmail = ticket.Customer?.Email ?? string.Empty,
            AssignedEmployeeId = ticket.AssignedEmployeeId,
            DepartmentId = ticket.DepartmentId,
            CreatedAt = ticket.CreatedAt,
            SlaStartTime = ticket.SlaStartTime,
            IsSlaBreached = ticket.IsSlaBreached
        };
    }

    public async Task<IEnumerable<TicketResponseDto>> GetAllTicketsAsync()
    {
        var tickets = await _context.Tickets
            .Include(t => t.Customer)
            .ToListAsync();

        return tickets.Select(ticket => new TicketResponseDto
        {
            Id = ticket.Id,
            Title = ticket.Title,
            Description = ticket.Description,
            Status = ticket.Status.ToString(),
            CustomerId = ticket.CustomerId,
            CustomerName = ticket.Customer?.Name ?? string.Empty,
            CustomerEmail = ticket.Customer?.Email ?? string.Empty,
            AssignedEmployeeId = ticket.AssignedEmployeeId,
            DepartmentId = ticket.DepartmentId,
            CreatedAt = ticket.CreatedAt,
            SlaStartTime = ticket.SlaStartTime,
            IsSlaBreached = ticket.IsSlaBreached
        });
    }

    public async Task<Ticket?> UpdateTicketAsync(Ticket ticket)
    {
        return null;
    }
}
