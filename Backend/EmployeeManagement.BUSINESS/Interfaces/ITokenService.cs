using System.Threading.Tasks;
using EmployeeManagement.BUSINESS.BusinessModels.RequestDTOs.AuthRequestDTOs;
using EmployeeManagement.BUSINESS.BusinessModels.ResponseDTOs.AuthResponseDTOs;

namespace EmployeeManagement.BUSINESS.Interfaces
{
	public interface ITokenService
	{
		Task<AuthResponseDto?> AuthenticateAsync(LoginRequestDto request);

		Task<AuthResponseDto?> RefreshTokenAsync(string token);

		Task<AuthResponseDto?> RegisterAsync(RegisterRequestDto request);

		Task<bool> RevokeTokenAsync(string token);
	}
}