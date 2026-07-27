using EmployeeManagement.DATA.Contexts;
using EmployeeManagement.DATA.DomainModels.AuthDATA;
using EmployeeManagement.DATA.Interfaces.IRepositories;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace EmployeeManagement.DATA.Implementations.Repositories
{
    public class AuthRepository : GenericRepository<RefreshToken>, IAuthRepository
    {
        public AuthRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<RefreshToken> GetByTokenAsync(string token)
        {
            return await _dbSet.SingleOrDefaultAsync(rt => rt.Token == token);
        }
    }
}
