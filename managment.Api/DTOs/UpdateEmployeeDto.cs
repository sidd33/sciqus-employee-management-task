using System.ComponentModel.DataAnnotations;

namespace managment.Api.DTOs;

public record UpdateEmployeeDto(
    [Required(ErrorMessage = "First name is required.")]
    [StringLength(50)]
    string FirstName,

    [Required(ErrorMessage = "Last name is required.")]
    [StringLength(50)]
    string LastName,

    [StringLength(50)]
    string? Department
);
