using managment.Api.Data;
using managment.Api.DTOs;
using managment.Api.Entities;
using Microsoft.EntityFrameworkCore;

namespace managment.Api.Services;

public class EmployeeService : IEmployeeService
{
    private readonly AppDbContext _context;

    // EF Core DbContext is injected automatically by ASP.NET Core DI
    public EmployeeService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<EmployeeResponseDto>> GetAllAsync()
    {
        return await _context.Employees
            .AsNoTracking() // Performance optimization for read-only queries
            .Select(e => MapToResponseDto(e))
            .ToListAsync();
    }

    public async Task<EmployeeResponseDto?> GetByIdAsync(Guid id)
    {
        var employee = await _context.Employees.FindAsync(id);
        return employee == null ? null : MapToResponseDto(employee);
    }

    public async Task<EmployeeResponseDto> CreateAsync(CreateEmployeeDto dto)
    {
        var newEmployee = new Employee
        {
            Id = Guid.NewGuid(),
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            Email = dto.Email,
            Department = dto.Department,
            Role = dto.Role,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        _context.Employees.Add(newEmployee);
        await _context.SaveChangesAsync();

        return MapToResponseDto(newEmployee);
    }

    public async Task<EmployeeResponseDto?> UpdateAsync(Guid id, UpdateEmployeeDto dto)
    {
        var existing = await _context.Employees.FindAsync(id);
        if (existing == null)
        {
            return null;
        }

        existing.FirstName = dto.FirstName;
        existing.LastName = dto.LastName;
        existing.Department = dto.Department;
        existing.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return MapToResponseDto(existing);
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var employee = await _context.Employees.FindAsync(id);
        if (employee == null)
        {
            return false;
        }

        _context.Employees.Remove(employee);
        await _context.SaveChangesAsync();
        return true;
    }

    private static EmployeeResponseDto MapToResponseDto(Employee emp)
    {
        return new EmployeeResponseDto(
            emp.Id,
            emp.FirstName,
            emp.LastName,
            emp.Email,
            emp.Department,
            emp.Role,
            emp.IsActive,
            emp.CreatedAt
        );
    }
}
