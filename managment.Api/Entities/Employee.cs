namespace managment.Api.Entities;

public class Employee
{
    public Guid Id { get; set; } = Guid.NewGuid();
    
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Department { get; set; }
    
    public EmployeeRole Role { get; set; } = EmployeeRole.Employee;
    
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
}
