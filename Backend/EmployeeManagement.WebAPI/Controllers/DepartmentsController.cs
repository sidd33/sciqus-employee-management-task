using System;
using System.Threading.Tasks;

using EmployeeManagement.BUSINESS.BusinessModels.RequestDTOs.DepartmentRequestDtos;
using EmployeeManagement.BUSINESS.Interfaces.IService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeManagement.WebAPI.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	// Authentication required for all actions.
	// Admin-only restriction applied per-action below, not controller-wide,
	// since employees should be able to view departments.
	[Authorize]
	public class DepartmentsController : ControllerBase
	{
		private readonly IDepartmentService _departmentService;

		public DepartmentsController(IDepartmentService departmentService)
		{
			_departmentService = departmentService;
		}

		// GET: api/departments
		// Any authenticated user (Admin or Employee) can view departments.
		[HttpGet]
		[ProducesResponseType(StatusCodes.Status200OK)]
		public async Task<IActionResult> GetAll([FromQuery] DepartmentQueryParameters query)
		{
			var result = await _departmentService.GetAllAsync(query);
			return Ok(result);
		}

		// GET: api/departments/{id}
		[HttpGet("{id:guid}")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		public async Task<IActionResult> GetById(Guid id)
		{
			var department = await _departmentService.GetByIdAsync(id);

			if (department == null)
				return NotFound();

			return Ok(department);
		}

		// POST: api/departments
		// Admin only (BR-001).
		[HttpPost]
		[Authorize(Roles = "Admin,SuperAdmin")]
		[ProducesResponseType(StatusCodes.Status201Created)]
		[ProducesResponseType(StatusCodes.Status409Conflict)]
		public async Task<IActionResult> Create([FromBody] CreateDepartmentDto dto)
		{
			try
			{
				var created = await _departmentService.CreateAsync(dto);
				return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
			}
			catch (InvalidOperationException ex)
			{
				return Conflict(new { message = ex.Message });
			}
		}

		// PUT: api/departments/{id}
		// Admin only (BR-001).
		[HttpPut("{id:guid}")]
		[Authorize(Roles = "Admin,SuperAdmin")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		[ProducesResponseType(StatusCodes.Status409Conflict)]
		public async Task<IActionResult> Update(Guid id, [FromBody] UpdateDepartmentDto dto)
		{
			try
			{
				var updated = await _departmentService.UpdateAsync(id, dto);

				if (updated == null)
					return NotFound();

				return Ok(updated);
			}
			catch (InvalidOperationException ex)
			{
				return Conflict(new { message = ex.Message });
			}
		}
	}
}