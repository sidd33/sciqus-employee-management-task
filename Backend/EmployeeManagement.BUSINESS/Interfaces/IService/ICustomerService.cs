namespace EmployeeManagement.BUSINESS.Interfaces.IService;

using EmployeeManagement.DATA.DomainModels.CustomerDATA;

public interface ICustomerService
{
    Task<Customer?> GetCustomerByIdAsync(Guid id);
    Task<IEnumerable<Customer>> GetAllCustomersAsync();
    Task<Customer?> UpdateCustomerAsync(Customer customer);
    Task<bool> DeleteCustomerAsync(Guid id);
}
