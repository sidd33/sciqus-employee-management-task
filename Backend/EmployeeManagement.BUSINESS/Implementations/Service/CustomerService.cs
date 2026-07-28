namespace EmployeeManagement.BUSINESS.Implementations.Service;

using Microsoft.EntityFrameworkCore;
using EmployeeManagement.BUSINESS.BusinessModels.RequestDTOs.CustomerRequestDtos;
using EmployeeManagement.BUSINESS.BusinessModels.ResponseDTOs.CustomerResponseDtos;
using EmployeeManagement.BUSINESS.BusinessModels.ResponseDTOs.TicketResponseDtos;
using EmployeeManagement.BUSINESS.Interfaces.IService;
using EmployeeManagement.DATA.Contexts;

public class CustomerService : ICustomerService
{
    private readonly AppDbContext _context;

    public CustomerService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<CustomerResponseDto?> GetCustomerByIdAsync(Guid id)
    {
        var customer = await _context.Customers
            .Include(c => c.Tickets)
            .FirstOrDefaultAsync(c => c.Id == id);

        if (customer == null) return null;

        return new CustomerResponseDto
        {
            Id = customer.Id,
            Name = customer.Name,
            Email = customer.Email,
            ProfilePicture = customer.ProfilePicture,
            Tickets = customer.Tickets.Select(t => new TicketResponseDto
            {
                Id = t.Id,
                Title = t.Title,
                Description = t.Description,
                Status = t.Status.ToString(),
                CustomerId = t.CustomerId,
                AssignedEmployeeId = t.AssignedEmployeeId,
                DepartmentId = t.DepartmentId,
                CreatedAt = t.CreatedAt,
                SlaStartTime = t.SlaStartTime,
                IsSlaBreached = t.IsSlaBreached
            }).ToList()
        };
    }

    public async Task<IEnumerable<CustomerResponseDto>> GetAllCustomersAsync()
    {
        var customers = await _context.Customers
            .Include(c => c.Tickets)
            .ToListAsync();

        return customers.Select(c => new CustomerResponseDto
        {
            Id = c.Id,
            Name = c.Name,
            Email = c.Email,
            ProfilePicture = c.ProfilePicture,
            Tickets = c.Tickets.Select(t => new TicketResponseDto
            {
                Id = t.Id,
                Title = t.Title,
                Description = t.Description,
                Status = t.Status.ToString(),
                CustomerId = t.CustomerId,
                AssignedEmployeeId = t.AssignedEmployeeId,
                DepartmentId = t.DepartmentId,
                CreatedAt = t.CreatedAt,
                SlaStartTime = t.SlaStartTime,
                IsSlaBreached = t.IsSlaBreached
            }).ToList()
        });
    }

    public async Task<CustomerResponseDto?> UpdateCustomerAsync(Guid id, UpdateCustomerDto dto)
    {
        var customer = await _context.Customers
            .Include(c => c.Tickets)
            .FirstOrDefaultAsync(c => c.Id == id);

        if (customer == null) return null;

        if (!string.IsNullOrWhiteSpace(dto.Name)) customer.Name = dto.Name;
        if (!string.IsNullOrWhiteSpace(dto.Email)) customer.Email = dto.Email;
        if (dto.ProfilePicture != null) customer.ProfilePicture = dto.ProfilePicture;

        await _context.SaveChangesAsync();

        return new CustomerResponseDto
        {
            Id = customer.Id,
            Name = customer.Name,
            Email = customer.Email,
            ProfilePicture = customer.ProfilePicture,
            Tickets = customer.Tickets.Select(t => new TicketResponseDto
            {
                Id = t.Id,
                Title = t.Title,
                Description = t.Description,
                Status = t.Status.ToString(),
                CustomerId = t.CustomerId,
                AssignedEmployeeId = t.AssignedEmployeeId,
                DepartmentId = t.DepartmentId,
                CreatedAt = t.CreatedAt,
                SlaStartTime = t.SlaStartTime,
                IsSlaBreached = t.IsSlaBreached
            }).ToList()
        };
    }

    public async Task<bool> DeleteCustomerAsync(Guid id)
    {
        var customer = await _context.Customers.FindAsync(id);
        if (customer == null) return false;

        _context.Customers.Remove(customer);
        await _context.SaveChangesAsync();
        return true;
    }
}
