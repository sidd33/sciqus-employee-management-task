namespace EmployeeManagement.BUSINESS.BusinessModels.RequestDTOs.CustomerRequestDtos;

using System.ComponentModel.DataAnnotations;

public class UpdateCustomerDto
{
    public string? Name { get; set; }

    [EmailAddress(ErrorMessage = "Invalid email address format.")]
    public string? Email { get; set; }

    public string? ProfilePicture { get; set; }
}
