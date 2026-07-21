using managment.Api.DTOs;
using managment.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace managment.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EmployeesController : ControllerBase
{
    private readonly IEmployeeService _employeeService;

    public EmployeesController(IEmployeeService employeeService)
    {
        _employeeService = employeeService;
    }

 
    [HttpGet]
    public async Task<ActionResult<IEnumerable<EmployeeResponseDto>>> GetAll()
    {
        var employees = await _employeeService.GetAllAsync();
        return Ok(employees);
    }

 
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<EmployeeResponseDto>> GetById(Guid id)
    {
        var employee = await _employeeService.GetByIdAsync(id);
        if (employee == null)
        {
            return NotFound(new { message = $"Employee with ID '{id}' was not found." });
        }

        return Ok(employee);
    }

 
    [HttpPost]
    public async Task<ActionResult<EmployeeResponseDto>> Create([FromBody] CreateEmployeeDto dto)
    {
        var created = await _employeeService.CreateAsync(dto);
 
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }
 
    [HttpPut("{id:guid}")]
    public async Task<ActionResult<EmployeeResponseDto>> Update(Guid id, [FromBody] UpdateEmployeeDto dto)
    {
        var updated = await _employeeService.UpdateAsync(id, dto);
        if (updated == null)
        {
            return NotFound(new { message = $"Employee with ID '{id}' was not found." });
        }

        return Ok(updated);
    }

 
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var success = await _employeeService.DeleteAsync(id);
        if (!success)
        {
            return NotFound(new { message = $"Employee with ID '{id}' was not found." });
        }

        return NoContent();  
    }
}
