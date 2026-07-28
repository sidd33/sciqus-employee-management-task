namespace EmployeeManagement.WebAPI.Controllers;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using EmployeeManagement.BUSINESS.BusinessModels.RequestDTOs.CustomerRequestDtos;
using EmployeeManagement.BUSINESS.Interfaces.IService;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class CustomersController : ControllerBase
{
    private readonly ICustomerService _customerService;

    public CustomersController(ICustomerService customerService)
    {
        _customerService = customerService;
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetCustomerById(Guid id)
    {
        var currentUserId = User.FindFirstValue(System.Security.Claims.ClaimTypes.NameIdentifier);
        var currentUserRole = User.FindFirstValue(System.Security.Claims.ClaimTypes.Role);

        if (currentUserRole != "Admin" && currentUserRole != "SuperAdmin" && currentUserId != id.ToString())
        {
            return Forbid();
        }

        var customer = await _customerService.GetCustomerByIdAsync(id);
        if (customer == null) return NotFound();
        return Ok(customer);
    }

    [HttpGet]
    [Authorize(Roles = "Admin,SuperAdmin")]
    public async Task<IActionResult> GetAllCustomers([FromQuery] CustomerQueryParameters query)
    {
        var customers = await _customerService.GetAllCustomersAsync(query);
        return Ok(customers);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateCustomer(Guid id, [FromBody] UpdateCustomerDto dto)
    {
        var currentUserId = User.FindFirstValue(System.Security.Claims.ClaimTypes.NameIdentifier);
        var currentUserRole = User.FindFirstValue(System.Security.Claims.ClaimTypes.Role);

        if (currentUserRole != "Admin" && currentUserRole != "SuperAdmin" && currentUserId != id.ToString())
        {
            return Forbid();
        }

        try
        {
            var updatedCustomer = await _customerService.UpdateCustomerAsync(id, dto);
            if (updatedCustomer == null) return NotFound();
            return Ok(updatedCustomer);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Admin,SuperAdmin")]
    public async Task<IActionResult> DeleteCustomer(Guid id)
    {
        var deleted = await _customerService.DeleteCustomerAsync(id);
        if (!deleted) return NotFound();
        return NoContent();
    }
}
