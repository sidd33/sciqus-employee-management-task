using System.ComponentModel.DataAnnotations;
using managment.Api.Entities;

namespace managment.Api.DTOs;

public record CreateEmployeeDto(
    [Required(ErrorMessage = "First name is required.")]
    [StringLength(50, ErrorMessage = "First name cannot exceed 50 characters.")]
    string FirstName,

    [Required(ErrorMessage = "Last name is required.")]
    [StringLength(50, ErrorMessage = "Last name cannot exceed 50 characters.")]
    string LastName,

    [Required(ErrorMessage = "Email address is required.")]
    [EmailAddress(ErrorMessage = "Invalid email format.")]
    string Email,

    [StringLength(50)]
    string? Department,

    [Required]
    EmployeeRole Role = EmployeeRole.Employee
);
