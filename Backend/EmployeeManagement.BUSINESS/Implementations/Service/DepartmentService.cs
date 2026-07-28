using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AutoMapper;
using EmployeeManagement.BUSINESS.BusinessModels;
using EmployeeManagement.BUSINESS.BusinessModels.RequestDTOs.DepartmentRequestDtos;
using EmployeeManagement.BUSINESS.BusinessModels.ResponseDTOs.DepartmentResponseDtos;
using EmployeeManagement.BUSINESS.Interfaces.IService;
using EmployeeManagement.DATA.Contexts;
using EmployeeManagement.DATA.DomainModels.DepartmentDATA;
using Microsoft.EntityFrameworkCore;

namespace EmployeeManagement.BUSINESS.Implementations.Service
{
	public class DepartmentService : IDepartmentService
	{
		// Database Context
		// Used for interacting with the database.
		private readonly AppDbContext _context;

		// AutoMapper
		// Converts Entity <-> DTO.
		private readonly IMapper _mapper;

		// Dependency Injection Constructor
		public DepartmentService(AppDbContext context, IMapper mapper)
		{
			_context = context;
			_mapper = mapper;
		}

		// =====================================================
		// GET ALL DEPARTMENTS
		//
		// Supports:
		// 1. Search
		// 2. Pagination
		// 3. Employee Count
		// =====================================================
		public async Task<PagedResponse<DepartmentResponseDto>> GetAllAsync(
			DepartmentQueryParameters query)
		{
			// Build base query.
			// Include Employees because we need EmployeeCount.
			var departmentsQuery = _context.Departments
				.Include(d => d.Employees)
				.AsQueryable();

			// ---------------------------------------------
			// Search by Department Name
			// Example:
			// Search = "HR"
			// Returns:
			// HR
			// Human Resources
			// ---------------------------------------------
			if (!string.IsNullOrWhiteSpace(query.SearchTerm))
			{
				departmentsQuery = departmentsQuery.Where(d =>
					d.Name.Contains(query.SearchTerm));
			}

			// Total number of departments before pagination.
			var totalCount = await departmentsQuery.CountAsync();

			// Apply sorting and pagination.
			var departments = await departmentsQuery
				.OrderBy(d => d.Name)
				.Skip((query.PageNumber - 1) * query.PageSize)
				.Take(query.PageSize)
				.ToListAsync();

			// Convert each Department entity into a DTO.
			// Also populate EmployeeCount.
			var items = departments.Select(d =>
			{
				var dto = _mapper.Map<DepartmentResponseDto>(d);

				dto.EmployeeCount = d.Employees.Count;

				return dto;
			});

			// Return paginated response.
			return new PagedResponse<DepartmentResponseDto>
			{
				Items = items,
				PageNumber = query.PageNumber,
				PageSize = query.PageSize,
				TotalCount = totalCount
			};
		}

		// =====================================================
		// GET DEPARTMENT BY ID
		//
		// Returns null if department does not exist.
		// =====================================================
		public async Task<DepartmentResponseDto?> GetByIdAsync(Guid id)
		{
			// Fetch department along with its employees.
			var department = await _context.Departments
				.Include(d => d.Employees)
				.FirstOrDefaultAsync(d => d.Id == id);

			if (department == null)
				return null;

			// Convert Entity -> DTO.
			var dto = _mapper.Map<DepartmentResponseDto>(department);

			// Number of employees in this department.
			dto.EmployeeCount = department.Employees.Count;

			return dto;
		}

		// =====================================================
		// CREATE NEW DEPARTMENT
		//
		// Business Rules:
		// 1. Department name must be unique.
		// =====================================================
		public async Task<DepartmentResponseDto> CreateAsync(
			CreateDepartmentDto dto)
		{
			// ---------------------------------------------
			// Check whether a department with the same name
			// already exists.
			// ---------------------------------------------
			var departmentExists = await _context.Departments
				.AnyAsync(d => d.Name.ToLower() == dto.Name.ToLower());

			if (departmentExists)
			{
				throw new InvalidOperationException(
					"Department already exists.");
			}

			// Convert DTO -> Entity.
			var department = _mapper.Map<Department>(dto);

			// CreatedAt is usually set automatically in the entity,
			// but setting it explicitly is also acceptable.
			department.CreatedAt = DateTime.UtcNow;

			// Save department.
			_context.Departments.Add(department);

			await _context.SaveChangesAsync();

			return _mapper.Map<DepartmentResponseDto>(department);
		}

		// =====================================================
		// UPDATE DEPARTMENT
		//
		// Business Rules:
		// 1. Department must exist.
		// 2. New name should not duplicate another department.
		// =====================================================
		public async Task<DepartmentResponseDto?> UpdateAsync(
			Guid id,
			UpdateDepartmentDto dto)
		{
			// Find department.
			var department = await _context.Departments
				.FindAsync(id);

			if (department == null)
				return null;

			// ---------------------------------------------
			// Prevent duplicate department names.
			// Ignore the current department while checking.
			// ---------------------------------------------
			var departmentExists = await _context.Departments
				.AnyAsync(d =>
					d.Name.ToLower() == dto.Name.ToLower() &&
					d.Id != id);

			if (departmentExists)
			{
				throw new InvalidOperationException(
					"Department already exists.");
			}

			// Update editable fields.
			department.Name = dto.Name;

			department.Description = dto.Description;

			department.UpdatedAt = DateTime.UtcNow;

			await _context.SaveChangesAsync();

			return _mapper.Map<DepartmentResponseDto>(department);
		}
	}
}
