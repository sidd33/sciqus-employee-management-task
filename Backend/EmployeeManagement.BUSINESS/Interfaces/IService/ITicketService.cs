namespace EmployeeManagement.BUSINESS.Interfaces.IService;

using EmployeeManagement.BUSINESS.BusinessModels.RequestDTOs.TicketRequestDtos;
using EmployeeManagement.BUSINESS.BusinessModels.ResponseDTOs.TicketResponseDtos;

public interface ITicketService
{
    Task<TicketResponseDto> CreateTicketAsync(CreateTicketDto dto);
    Task<TicketResponseDto?> GetTicketByIdAsync(Guid id);
    Task<IEnumerable<TicketResponseDto>> GetAllTicketsAsync(TicketQueryParameters query);
    Task<TicketResponseDto?> UpdateTicketAsync(Guid id, UpdateTicketDto dto);
    Task<TicketResponseDto?> AssignTicketAsync(Guid id, AssignTicketDto dto);
}
