musing managment.Api.Entities;

namespace managment.Api.DTOs;

public record EmployeeResponseDto(
    Guid Id,
    string FirstName,
    string LastName,
    string Email,
    string? Department,
    EmployeeRole Role,
    bool IsActive,
    DateTime CreatedAt
);
