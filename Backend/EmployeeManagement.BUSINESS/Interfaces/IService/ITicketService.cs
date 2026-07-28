namespace EmployeeManagement.BUSINESS.Interfaces.IService;

using EmployeeManagement.BUSINESS.BusinessModels.RequestDTOs.TicketRequestDtos;
using EmployeeManagement.BUSINESS.BusinessModels.ResponseDTOs.TicketResponseDtos;
using EmployeeManagement.DATA.DomainModels.TicketDATA;

public interface ITicketService
{
    Task<Ticket> CreateTicketAsync(CreateTicketDto dto);
    Task<TicketResponseDto?> GetTicketByIdAsync(Guid id);
    Task<IEnumerable<TicketResponseDto>> GetAllTicketsAsync();
    Task<TicketResponseDto?> UpdateTicketAsync(Guid id, UpdateTicketDto dto);
}
