using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using EmployeeManagement.DATA.Contexts;
using EmployeeManagement.DATA.DomainModels.AuthDATA;
using EmployeeManagement.DATA.Interfaces.IRepositories;
using EmployeeManagement.BUSINESS.Interfaces;
using EmployeeManagement.BUSINESS.BusinessModels.RequestDTOs.AuthRequestDTOs;
using EmployeeManagement.BUSINESS.BusinessModels.ResponseDTOs.AuthResponseDTOs;

namespace EmployeeManagement.BUSINESS.Implementations
{
    public class TokenService : ITokenService
    {
        private readonly AppDbContext _context;
        private readonly IAuthRepository _authRepository;

        // In a real app, this should be in appsettings.json!
        private readonly string _secretKey = "SuperSecretKeyForDevelopment12345!@#SuperSecretKeyForDevelopment12345!@#"; 

        public TokenService(AppDbContext context, IAuthRepository authRepository)
        {
            _context = context;
            _authRepository = authRepository;
        }

        public async Task<AuthResponseDto?> AuthenticateAsync(LoginRequestDto request)
        {
			// 1. Check if it's an Employee
			var employee = await _context.Set<EmployeeManagement.DATA.DomainModels.EmployeeDATA.Employee>()
	.FirstOrDefaultAsync(e => e.Email == request.Email);

			string userId = string.Empty;

            if (employee != null && BCrypt.Net.BCrypt.Verify(request.Password, employee.PasswordHash))
                {
                    userId = employee.Id.ToString();
                }
            else
            {
                // 2. If not an Employee, check if it's a Customer by Email only
                var customer = await _context.Set<EmployeeManagement.DATA.DomainModels.CustomerDATA.Customer>()
        .FirstOrDefaultAsync(c => c.Email == request.Email);
    // If customer exists AND the password matches the hash
    if (customer != null && BCrypt.Net.BCrypt.Verify(request.Password, customer.Password))
    {
        userId = customer.Id.ToString();
    }
    else
    {
        // Invalid email or password!
        return null; 
    }
            }

            // 3. Generate the JWT (Access Token)
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_secretKey);

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, userId),
                new Claim(JwtRegisteredClaimNames.Email, request.Email)
            };

            if (employee != null)
            {
                claims.Add(new Claim(ClaimTypes.Role, employee.Role.ToString()));
            }
            else
            {
                claims.Add(new Claim(ClaimTypes.Role, "Customer"));
            }

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(15), // Access token lives for 15 minutes
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            var jwtString = tokenHandler.WriteToken(token);

            // 4. Generate a Refresh Token
            var refreshToken = new RefreshToken
            {
                Token = Guid.NewGuid().ToString(),
                UserId = userId,
                Expires = DateTime.UtcNow.AddDays(7) // Refresh token lives for 7 days
            };

            await _authRepository.AddAsync(refreshToken);
            await _authRepository.SaveChangesAsync();

            // 5. Return the boxes
            return new AuthResponseDto
            {
                AccessToken = jwtString,
                RefreshToken = refreshToken.Token
            };
        }

        public async Task<AuthResponseDto?> RefreshTokenAsync(string token)
        {
            var existingToken = await _authRepository.GetByTokenAsync(token);

            if (existingToken == null || existingToken.IsExpired)
            {
                return null;
            }

            // Look up the user to get their role and email
            var employee = await _context.Set<EmployeeManagement.DATA.DomainModels.EmployeeDATA.Employee>()
                .FirstOrDefaultAsync(e => e.Id.ToString() == existingToken.UserId);

            var customer = await _context.Set<EmployeeManagement.DATA.DomainModels.CustomerDATA.Customer>()
                .FirstOrDefaultAsync(c => c.Id.ToString() == existingToken.UserId);

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, existingToken.UserId)
            };

            if (employee != null)
            {
                claims.Add(new Claim(JwtRegisteredClaimNames.Email, employee.Email));
                claims.Add(new Claim(ClaimTypes.Role, employee.Role.ToString()));
            }
            else if (customer != null)
            {
                claims.Add(new Claim(JwtRegisteredClaimNames.Email, customer.Email));
                claims.Add(new Claim(ClaimTypes.Role, "Customer"));
            }

            // Generate a brand new access token for this user
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_secretKey);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(15),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var newJwt = tokenHandler.CreateToken(tokenDescriptor);

            return new AuthResponseDto
            {
                AccessToken = tokenHandler.WriteToken(newJwt),
                RefreshToken = existingToken.Token
            };
        }

        public async Task<AuthResponseDto?> RegisterAsync(RegisterRequestDto request)
        {
            // 1. Check if a Customer with this email already exists
            var existingCustomer = await _context.Set<EmployeeManagement.DATA.DomainModels.CustomerDATA.Customer>()
                .FirstOrDefaultAsync(c => c.Email == request.Email);

            if (existingCustomer != null)
            {
                // Email is already taken! 
                return null; 
            }

            // 2. Create the new Customer
            var customer = new EmployeeManagement.DATA.DomainModels.CustomerDATA.Customer
            {
                Id = Guid.NewGuid(),
                Name = request.Name,
                Email = request.Email,
                Password = BCrypt.Net.BCrypt.HashPassword(request.Password) 
            };

            // 3. Save to SQL Database
            await _context.Set<EmployeeManagement.DATA.DomainModels.CustomerDATA.Customer>().AddAsync(customer);
            await _context.SaveChangesAsync();

            // 4. Generate the JWT for their instant login
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_secretKey);

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, customer.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, customer.Email),
                new Claim(ClaimTypes.Role, "Customer")
            };

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(15), 
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            var jwtString = tokenHandler.WriteToken(token);

            // 5. Generate a Refresh Token
            var refreshToken = new RefreshToken
            {
                Token = Guid.NewGuid().ToString(),
                UserId = customer.Id.ToString(),
                Expires = DateTime.UtcNow.AddDays(7) 
            };

            await _authRepository.AddAsync(refreshToken);
            await _authRepository.SaveChangesAsync();

            // 6. Return the box
            return new AuthResponseDto
            {
                AccessToken = jwtString,
                RefreshToken = refreshToken.Token
            };
        }

        public async Task<bool> RevokeTokenAsync(string token)
        {
    
            var existingToken = await _authRepository.GetByTokenAsync(token);

            if (existingToken == null)
            {
                return false; 
            }
            
            _authRepository.Delete(existingToken);
            await _authRepository.SaveChangesAsync();

        return true;
        }

    }
}
