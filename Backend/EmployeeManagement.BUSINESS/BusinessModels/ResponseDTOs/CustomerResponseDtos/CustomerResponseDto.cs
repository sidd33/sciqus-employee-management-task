namespace EmployeeManagement.BUSINESS.BusinessModels.ResponseDTOs.CustomerResponseDtos;

using EmployeeManagement.BUSINESS.BusinessModels.ResponseDTOs.TicketResponseDtos;

public class CustomerResponseDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? ProfilePicture { get; set; }
    public ICollection<TicketResponseDto> Tickets { get; set; } = new List<TicketResponseDto>();
}
