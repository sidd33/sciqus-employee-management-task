using System.Collections.Concurrent;
using managment.Api.DTOs;
using managment.Api.Entities;

namespace managment.Api.Services;

public class EmployeeService : IEmployeeService
{
    // Thread-safe dictionary to simulate in-memory database
    private readonly ConcurrentDictionary<Guid, Employee> _employees = new();

    public EmployeeService()
    {
        // Seed initial data for testing
        var admin = new Employee
        {
            Id = Guid.Parse("11111111-1111-1111-1111-111111111111"),
            FirstName = "Alice",
            LastName = "Manager",
            Email = "alice.manager@company.com",
            Department = "Engineering Management",
            Role = EmployeeRole.Admin,
            CreatedAt = DateTime.UtcNow
        };

        var employee = new Employee
        {
            Id = Guid.Parse("22222222-2222-2222-2222-222222222222"),
            FirstName = "Bob",
            LastName = "Developer",
            Email = "bob.dev@company.com",
            Department = "Software Engineering",
            Role = EmployeeRole.Employee,
            CreatedAt = DateTime.UtcNow
        };

        _employees.TryAdd(admin.Id, admin);
        _employees.TryAdd(employee.Id, employee);
    }

    public Task<IEnumerable<EmployeeResponseDto>> GetAllAsync()
    {
        var response = _employees.Values.Select(MapToResponseDto);
        return Task.FromResult(response);
    }

    public Task<EmployeeResponseDto?> GetByIdAsync(Guid id)
    {
        if (_employees.TryGetValue(id, out var employee))
        {
            return Task.FromResult<EmployeeResponseDto?>(MapToResponseDto(employee));
        }

        return Task.FromResult<EmployeeResponseDto?>(null);
    }

    public Task<EmployeeResponseDto> CreateAsync(CreateEmployeeDto dto)
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

        _employees.TryAdd(newEmployee.Id, newEmployee);
        return Task.FromResult(MapToResponseDto(newEmployee));
    }

    public Task<EmployeeResponseDto?> UpdateAsync(Guid id, UpdateEmployeeDto dto)
    {
        if (!_employees.TryGetValue(id, out var existing))
        {
            return Task.FromResult<EmployeeResponseDto?>(null);
        }

        existing.FirstName = dto.FirstName;
        existing.LastName = dto.LastName;
        existing.Department = dto.Department;
        existing.UpdatedAt = DateTime.UtcNow;

        return Task.FromResult<EmployeeResponseDto?>(MapToResponseDto(existing));
    }

    public Task<bool> DeleteAsync(Guid id)
    {
        var removed = _employees.TryRemove(id, out _);
        return Task.FromResult(removed);
    }

    // Private Helper method to convert Entity -> Response DTO
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
