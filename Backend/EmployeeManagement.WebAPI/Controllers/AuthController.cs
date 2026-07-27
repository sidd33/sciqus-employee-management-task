using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using EmployeeManagement.BUSINESS.Interfaces;
using EmployeeManagement.BUSINESS.BusinessModels.RequestDTOs.AuthRequestDTOs;

namespace EmployeeManagement.WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly ITokenService _tokenService;

        public AuthController(ITokenService tokenService)
        {
            _tokenService = tokenService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto request)
        {
            var response = await _tokenService.AuthenticateAsync(request);

            if (response == null)
            {
                return Unauthorized(new { message = "Invalid email or password." });
            }

            return Ok(response);
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequestDto request)
        {
            var result = await _tokenService.RegisterAsync(request);

            if (result == null)
            {
                return BadRequest(new { message = "Failed to register user." });
            }

            return CreatedAtAction(nameof(Login), new { email = request.Email }, result);
        }

        [HttpPost("refresh")]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshRequestDto request)
        {
            var response = await _tokenService.RefreshTokenAsync(request.RefreshToken);

            if (response == null)
            {
                return Unauthorized(new { message = "Invalid or expired refresh token." });
            }

            return Ok(response);
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout([FromBody] RefreshRequestDto request)
        {
            var result = await _tokenService.RevokeTokenAsync(request.RefreshToken);

            if (!result)
            {
                return BadRequest(new { message = "Token not found." });
            }

            return Ok(new { message = "Successfully logged out!" });
        }
    }
}
