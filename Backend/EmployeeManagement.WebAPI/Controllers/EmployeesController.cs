using System;
using System.Threading.Tasks;

using EmployeeManagement.BUSINESS.BusinessModels.RequestDTOs.EmployeeRequestDtos;
using EmployeeManagement.BUSINESS.Interfaces.IService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeManagement.WebAPI.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	[Authorize]
	public class EmployeesController : ControllerBase
	{
		private readonly IEmployeeService _employeeService;

		public EmployeesController(IEmployeeService employeeService)
		{
			_employeeService = employeeService;
		}

		// GET: api/employees
		// Admin only — full listing/search of all employees.
		[HttpGet]
		[Authorize(Roles = "Admin,SuperAdmin")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		public async Task<IActionResult> GetAll([FromQuery] EmployeeQueryParameters query)
		{
			var result = await _employeeService.GetAllAsync(query);
			return Ok(result);
		}

		// GET: api/employees/{id}
		// Authentication required.
		// Ownership/role validation (self-or-admin) can be added later
		// via a SameUserOrAdmin policy — not implemented yet.
		[HttpGet("{id:guid}")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		public async Task<IActionResult> GetById(Guid id)
		{
			var employee = await _employeeService.GetByIdAsync(id);

			if (employee == null)
				return NotFound();

			return Ok(employee);
		}

		// POST: api/employees
		// Admin only — hiring/creating employees.
		[HttpPost]
		[Authorize(Roles = "Admin,SuperAdmin")]
		[ProducesResponseType(StatusCodes.Status201Created)]
		[ProducesResponseType(StatusCodes.Status409Conflict)]
		public async Task<IActionResult> Create([FromBody] CreateEmployeeDto dto)
		{
			try
			{
				var created = await _employeeService.CreateAsync(dto);
				return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
			}
			catch (InvalidOperationException ex)
			{
				return Conflict(new { message = ex.Message });
			}
		}

		// PUT: api/employees/{id}
		// Authentication required.
		// Ownership/role validation (self-or-admin) can be added later
		// via a SameUserOrAdmin policy — not implemented yet.
		[HttpPut("{id:guid}")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		[ProducesResponseType(StatusCodes.Status409Conflict)]
		public async Task<IActionResult> Update(Guid id, [FromBody] UpdateEmployeeDto dto)
		{
			var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
			var currentUserRole = User.FindFirstValue(ClaimTypes.Role);

			if (currentUserRole != "Admin" && currentUserRole != "SuperAdmin" && currentUserId != id.ToString())
			{
				return Forbid();
			}

			try
			{
				var updated = await _employeeService.UpdateAsync(id, dto);

				if (updated == null)
					return NotFound();

				return Ok(updated);
			}
			catch (InvalidOperationException ex)
			{
				return Conflict(new { message = ex.Message });
			}
		}

		// PUT: api/employees/{id}/department
		// Admin only — reassigning an employee's department.
		[HttpPut("{id:guid}/department")]
		[Authorize(Roles = "Admin,SuperAdmin")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		[ProducesResponseType(StatusCodes.Status409Conflict)]
		public async Task<IActionResult> UpdateDepartment(Guid id, [FromBody] UpdateEmployeeDepartmentDto dto)
		{
			try
			{
				var updated = await _employeeService.UpdateDepartmentAsync(id, dto);

				if (updated == null)
					return NotFound();

				return Ok(updated);
			}
			catch (InvalidOperationException ex)
			{
				return Conflict(new { message = ex.Message });
			}
		}

		// PUT: api/employees/{id}/role
		// Admin, SuperAdmin only.
		[HttpPut("{id:guid}/role")]
		[Authorize(Roles = "Admin,SuperAdmin")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		[ProducesResponseType(StatusCodes.Status401Unauthorized)]
		public async Task<IActionResult> UpdateRole(Guid id, [FromBody] UpdateEmployeeRoleDto dto)
		{
			try
			{
				var currentUserRole = User.FindFirstValue(ClaimTypes.Role) ?? "";
				var updated = await _employeeService.UpdateRoleAsync(id, dto, currentUserRole);

				if (updated == null)
					return NotFound();

				return Ok(updated);
			}
			catch (UnauthorizedAccessException ex)
			{
				return Unauthorized(new { message = ex.Message });
			}
		}

		// DELETE: api/employees/{id}
		// Admin only — soft delete.
		[HttpDelete("{id:guid}")]
		[Authorize(Roles = "Admin,SuperAdmin")]
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		public async Task<IActionResult> Delete(Guid id)
		{
			var deleted = await _employeeService.SoftDeleteAsync(id);

			if (!deleted)
				return NotFound();

			return NoContent();
		}

		// POST: api/employees/{id}/profile-picture
		// Authentication required.
		// Ownership/role validation (self-or-admin) can be added later
		// via a SameUserOrAdmin policy — not implemented yet.
		[HttpPost("{id:guid}/profile-picture")]
		[Consumes("multipart/form-data")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		public async Task<IActionResult> UploadProfilePicture(
	Guid id,
	[FromForm] UploadProfilePictureDto request)
		{
			var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
			var currentUserRole = User.FindFirstValue(ClaimTypes.Role);

			if (currentUserRole != "Admin" && currentUserRole != "SuperAdmin" && currentUserId != id.ToString())
			{
				return Forbid();
			}

			if (request.File == null || request.File.Length == 0)
				return BadRequest(new { message = "No file uploaded." });

			var path = await _employeeService.UploadProfilePictureAsync(id, request.File);

			if (path == null)
				return NotFound();

			return Ok(new { profilePicture = path });
		}
	}
}