namespace EmployeeManagement.BUSINESS.Interfaces.IService;

using EmployeeManagement.BUSINESS.BusinessModels.RequestDTOs.CustomerRequestDtos;
using EmployeeManagement.BUSINESS.BusinessModels.ResponseDTOs.CustomerResponseDtos;

public interface ICustomerService
{
    Task<CustomerResponseDto?> GetCustomerByIdAsync(Guid id);
    Task<IEnumerable<CustomerResponseDto>> GetAllCustomersAsync(CustomerQueryParameters query);
    Task<CustomerResponseDto?> UpdateCustomerAsync(Guid id, UpdateCustomerDto dto);
    Task<bool> DeleteCustomerAsync(Guid id);
}
