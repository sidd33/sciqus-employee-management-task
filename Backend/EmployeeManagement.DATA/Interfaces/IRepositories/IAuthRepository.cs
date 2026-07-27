using EmployeeManagement.DATA.DomainModels.AuthDATA;
using System.Threading.Tasks;

namespace EmployeeManagement.DATA.Interfaces.IRepositories
{
    public interface IAuthRepository : IGenericRepository<RefreshToken>
    {
        Task<RefreshToken> GetByTokenAsync(string token);
    }
}
