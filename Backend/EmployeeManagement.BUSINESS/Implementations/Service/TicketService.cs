namespace EmployeeManagement.BUSINESS.Implementations.Service;

using Microsoft.EntityFrameworkCore;
using EmployeeManagement.BUSINESS.BusinessModels.RequestDTOs.TicketRequestDtos;
using EmployeeManagement.BUSINESS.BusinessModels.ResponseDTOs.TicketResponseDtos;
using EmployeeManagement.BUSINESS.Interfaces.IService;
using EmployeeManagement.DATA.Contexts;
using EmployeeManagement.DATA.DomainModels.EmployeeDATA;
using EmployeeManagement.DATA.DomainModels.TicketDATA;

public class TicketService : ITicketService
{
    private readonly AppDbContext _context;

    public TicketService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<TicketResponseDto> CreateTicketAsync(CreateTicketDto dto)
    {
        var assignedEmployee = await _context.Employees
            .Where(e => e.DepartmentId == dto.DepartmentId 
                     && e.IsActive 
                     && !e.IsDeleted 
                     && e.Role != EmployeeRole.Admin)
            .OrderBy(e => e.LastAssignedTicketAt.HasValue)
            .ThenBy(e => e.LastAssignedTicketAt)
            .FirstOrDefaultAsync();

        if (assignedEmployee == null)
        {
            throw new InvalidOperationException("No active non-admin employees available in this department to assign the ticket.");
        }

        var customer = await _context.Customers.FirstOrDefaultAsync(c => c.Id == dto.CustomerId);

        var ticket = new Ticket
        {
            Title = dto.Title,
            Description = dto.Description,
            CustomerId = dto.CustomerId,
            DepartmentId = dto.DepartmentId,
            AssignedEmployeeId = assignedEmployee.Id,
            Status = TicketStatus.Assigned,
            SlaStartTime = DateTime.UtcNow.AddHours(4),
            CreatedAt = DateTime.UtcNow
        };

        assignedEmployee.LastAssignedTicketAt = DateTime.UtcNow;

        _context.Tickets.Add(ticket);
        await _context.SaveChangesAsync();

        return new TicketResponseDto
        {
            Id = ticket.Id,
            Title = ticket.Title,
            Description = ticket.Description,
            Status = ticket.Status.ToString(),
            CustomerId = ticket.CustomerId,
            CustomerName = customer?.Name ?? string.Empty,
            CustomerEmail = customer?.Email ?? string.Empty,
            AssignedEmployeeId = ticket.AssignedEmployeeId,
            DepartmentId = ticket.DepartmentId,
            CreatedAt = ticket.CreatedAt,
            SlaStartTime = ticket.SlaStartTime,
            IsSlaBreached = ticket.IsSlaBreached
        };
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

    public async Task<TicketResponseDto?> UpdateTicketAsync(Guid id, UpdateTicketDto dto)
    {
        var ticket = await _context.Tickets
            .Include(t => t.Customer)
            .FirstOrDefaultAsync(t => t.Id == id);

        if (ticket == null) return null;

        if (!string.IsNullOrWhiteSpace(dto.Title)) ticket.Title = dto.Title;
        if (!string.IsNullOrWhiteSpace(dto.Description)) ticket.Description = dto.Description;
        if (dto.Status.HasValue) ticket.Status = dto.Status.Value;
        if (dto.DepartmentId.HasValue) ticket.DepartmentId = dto.DepartmentId.Value;
        ticket.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

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
}
