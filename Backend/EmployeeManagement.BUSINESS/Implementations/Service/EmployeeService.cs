using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AutoMapper;
using BCrypt.Net;
using EmployeeManagement.BUSINESS.BusinessModels;
using EmployeeManagement.BUSINESS.BusinessModels.RequestDTOs.EmployeeRequestDtos;
using EmployeeManagement.BUSINESS.BusinessModels.ResponseDTOs.EmployeeResponseDtos;
using EmployeeManagement.BUSINESS.Interfaces.IService;
using EmployeeManagement.DATA.Contexts;
using EmployeeManagement.DATA.DomainModels.EmployeeDATA;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Hosting;

namespace EmployeeManagement.BUSINESS.Implementations.Service
{
	public class EmployeeService : IEmployeeService
	{
		// Database context used to perform CRUD operations
		private readonly AppDbContext _context;

		// AutoMapper is used to convert Entity <-> DTO
		private readonly IMapper _mapper;

		// Used for saving uploaded profile pictures
		private readonly IWebHostEnvironment _env;

		// Dependency Injection Constructor
		public EmployeeService(
			AppDbContext context,
			IMapper mapper,
			IWebHostEnvironment env)
		{
			_context = context;
			_mapper = mapper;
			_env = env;
		}

		// =====================================================
		// GET ALL EMPLOYEES
		// Supports:
		// 1. Search
		// 2. Department Filter
		// 3. Pagination
		// =====================================================
		public async Task<PagedResponse<EmployeeResponseDto>> GetAllAsync(EmployeeQueryParameters query)
		{
			// Build the base query.
			// Include Department so that DepartmentName can be returned.
			// Ignore soft deleted employees.
			var employeesQuery = _context.Employees
				.Include(e => e.Department)
				.Where(e => !e.IsDeleted)
				.AsQueryable();

			// -----------------------------
			// Filter by Department
			// -----------------------------
			if (query.DepartmentId.HasValue)
			{
				employeesQuery = employeesQuery
					.Where(e => e.DepartmentId == query.DepartmentId.Value);
			}

			// -----------------------------
			// Search by
			// First Name
			// Last Name
			// Email
			// -----------------------------
			if (!string.IsNullOrWhiteSpace(query.SearchTerm))
			{
				employeesQuery = employeesQuery.Where(e =>
					e.FirstName.Contains(query.SearchTerm) ||
					e.LastName.Contains(query.SearchTerm) ||
					e.Email.Contains(query.SearchTerm));
			}

			// Total records before pagination.
			// Used by frontend for page numbers.
			var totalCount = await employeesQuery.CountAsync();

			// Apply sorting + pagination
			var employees = await employeesQuery
				.OrderBy(e => e.FirstName)
				.Skip((query.PageNumber - 1) * query.PageSize)
				.Take(query.PageSize)
				.ToListAsync();

			// Return paginated response
			return new PagedResponse<EmployeeResponseDto>
			{
				Items = _mapper.Map<IEnumerable<EmployeeResponseDto>>(employees),
				PageNumber = query.PageNumber,
				PageSize = query.PageSize,
				TotalCount = totalCount
			};
		}

		// =====================================================
		// GET SINGLE EMPLOYEE
		// Returns null if employee does not exist
		// or has been soft deleted.
		// =====================================================
		public async Task<EmployeeResponseDto?> GetByIdAsync(Guid id)
		{
			var employee = await _context.Employees

				// Include Department information
				.Include(e => e.Department)

				// Fetch employee
				.FirstOrDefaultAsync(e =>
					e.Id == id &&
					!e.IsDeleted);

			// Return null if employee not found
			if (employee == null)
				return null;

			// Convert Entity -> DTO
			return _mapper.Map<EmployeeResponseDto>(employee);
		}
		// =====================================================
		// CREATE / HIRE EMPLOYEE
		// Business Rules:
		// 1. Department must exist.
		// 2. Email must be unique.
		// 3. Password is stored as a BCrypt hash.
		// 4. Employee is active by default.
		// =====================================================
		public async Task<EmployeeResponseDto> CreateAsync(CreateEmployeeDto dto)
		{
			// ---------------------------------------------
			// Check whether the selected department exists.
			// Prevents assigning employees to invalid departments.
			// ---------------------------------------------
			var departmentExists = await _context.Departments
				.AnyAsync(d => d.Id == dto.DepartmentId);

			if (!departmentExists)
				throw new InvalidOperationException("Department does not exist.");

			// ---------------------------------------------
			// Check if another active employee already uses
			// the same email address.
			//
			// Email should be unique because it is used for
			// login and identification.
			// ---------------------------------------------
			var emailExists = await _context.Employees
				.AnyAsync(e =>
					e.Email == dto.Email &&
					!e.IsDeleted);

			if (emailExists)
				throw new InvalidOperationException("Email already exists.");

			// ---------------------------------------------
			// Convert DTO -> Employee Entity
			// AutoMapper copies common properties.
			// ---------------------------------------------
			var employee = _mapper.Map<Employee>(dto);

			// ---------------------------------------------
			// System-generated values
			// These should never come from the client.
			// ---------------------------------------------
			employee.Id = Guid.NewGuid();

			employee.JoinedAt = DateTime.UtcNow;

			employee.UpdatedAt = DateTime.UtcNow;

			employee.IsActive = true;

			employee.IsDeleted = false;

			// ---------------------------------------------
			// Store password securely.
			//
			// NEVER save plain text passwords.
			// BCrypt generates a secure hash.
			// ---------------------------------------------
			employee.PasswordHash =
				BCrypt.Net.BCrypt.HashPassword(dto.Password);

			// ---------------------------------------------
			// Save employee into database.
			// ---------------------------------------------
			_context.Employees.Add(employee);

			await _context.SaveChangesAsync();

			// ---------------------------------------------
			// Load Department so AutoMapper can include
			// Department information in the response.
			// ---------------------------------------------
			await _context.Entry(employee)
				.Reference(e => e.Department)
				.LoadAsync();

			// Return DTO instead of Entity.
			return _mapper.Map<EmployeeResponseDto>(employee);
		}

		// =====================================================
		// UPDATE EMPLOYEE DETAILS
		//
		// Can update:
		// First Name
		// Last Name
		// Email
		// Active Status
		// =====================================================
		public async Task<EmployeeResponseDto?> UpdateAsync(
			Guid id,
			UpdateEmployeeDto dto)
		{
			// Fetch employee
			var employee = await _context.Employees
				.Include(e => e.Department)
				.FirstOrDefaultAsync(e =>
					e.Id == id &&
					!e.IsDeleted);

			if (employee == null)
				return null;

			// ---------------------------------------------
			// Prevent duplicate emails.
			//
			// Ignore the current employee while checking.
			// ---------------------------------------------
			var emailExists = await _context.Employees
				.AnyAsync(e =>
					e.Email == dto.Email &&
					e.Id != id &&
					!e.IsDeleted);

			if (emailExists)
				throw new InvalidOperationException("Email already exists.");

			// ---------------------------------------------
			// Update editable fields.
			// ---------------------------------------------
			employee.FirstName = dto.FirstName;

			employee.LastName = dto.LastName;

			employee.Email = dto.Email;

			employee.IsActive = dto.IsActive;

			employee.UpdatedAt = DateTime.UtcNow;

			await _context.SaveChangesAsync();

			return _mapper.Map<EmployeeResponseDto>(employee);
		}

		// =====================================================
		// CHANGE EMPLOYEE DEPARTMENT
		//
		// Business Rules:
		// Employee must exist.
		// Department must exist.
		// =====================================================
		public async Task<EmployeeResponseDto?> UpdateDepartmentAsync(
			Guid id,
			UpdateEmployeeDepartmentDto dto)
		{
			// Find employee
			var employee = await _context.Employees
				.Include(e => e.Department)
				.FirstOrDefaultAsync(e =>
					e.Id == id &&
					!e.IsDeleted);

			if (employee == null)
				return null;

			// ---------------------------------------------
			// Verify department exists.
			// ---------------------------------------------
			var departmentExists = await _context.Departments
				.AnyAsync(d => d.Id == dto.DepartmentId);

			if (!departmentExists)
				throw new InvalidOperationException("Department does not exist.");

			// ---------------------------------------------
			// Assign employee to the new department.
			// ---------------------------------------------
			employee.DepartmentId = dto.DepartmentId;

			employee.UpdatedAt = DateTime.UtcNow;

			await _context.SaveChangesAsync();

			// Reload Department navigation property.
			await _context.Entry(employee)
				.Reference(e => e.Department)
				.LoadAsync();

			return _mapper.Map<EmployeeResponseDto>(employee);
		}

		// =====================================================
		// UPDATE EMPLOYEE ROLE
		//
		// Business Rules:
		// SuperAdmins can assign any role.
		// Admins cannot assign Admin or SuperAdmin roles.
		// =====================================================
		public async Task<EmployeeResponseDto?> UpdateRoleAsync(
			Guid id,
			UpdateEmployeeRoleDto dto,
			string currentUserRole)
		{
			var employee = await _context.Employees
				.Include(e => e.Department)
				.FirstOrDefaultAsync(e => e.Id == id && !e.IsDeleted);

			if (employee == null)
				return null;

			if (currentUserRole == EmployeeRole.Admin.ToString() && 
				(dto.Role == EmployeeRole.Admin || dto.Role == EmployeeRole.SuperAdmin))
			{
				throw new UnauthorizedAccessException("Admins cannot grant Admin or SuperAdmin roles.");
			}

			employee.Role = dto.Role;
			employee.UpdatedAt = DateTime.UtcNow;

			await _context.SaveChangesAsync();

			return _mapper.Map<EmployeeResponseDto>(employee);
		}
		// =====================================================
		// SOFT DELETE EMPLOYEE
		//
		// Instead of permanently deleting the employee from the
		// database, we simply mark them as deleted.
		//
		// Benefits:
		// - Keeps historical records
		// - Prevents accidental data loss
		// - Can be restored later if needed
		// =====================================================
		public async Task<bool> SoftDeleteAsync(Guid id)
		{
			// Find employee by primary key.
			// FindAsync() is efficient because it first checks
			// EF Core's tracking cache before querying the database.
			var employee = await _context.Employees.FindAsync(id);

			// Employee not found or already deleted.
			if (employee == null || employee.IsDeleted)
				return false;

			// ---------------------------------------------
			// Soft Delete
			//
			// Instead of removing the record:
			// DELETE FROM Employees
			//
			// We simply update its status.
			// ---------------------------------------------
			employee.IsDeleted = true;

			employee.IsActive = false;

			employee.DeletedDate = DateTime.UtcNow;

			employee.UpdatedAt = DateTime.UtcNow;

			await _context.SaveChangesAsync();

			return true;
		}

		// =====================================================
		// UPLOAD PROFILE PICTURE
		//
		// Saves the uploaded image inside:
		// wwwroot/uploads/profile-pictures
		//
		// Stores only the relative path in the database.
		// =====================================================
		public async Task<string?> UploadProfilePictureAsync(
			Guid id,
			IFormFile file)
		{
			// Find employee.
			var employee = await _context.Employees.FindAsync(id);

			// Employee doesn't exist or has been deleted.
			if (employee == null || employee.IsDeleted)
				return null;

			// ---------------------------------------------
			// Create upload folder if it doesn't exist.
			//
			// Directory.CreateDirectory() is safe.
			// It creates the folder only if needed.
			// ---------------------------------------------
			var uploadsFolder = Path.Combine(
				_env.WebRootPath,
				"uploads",
				"profile-pictures");

			Directory.CreateDirectory(uploadsFolder);

			// ---------------------------------------------
			// Generate a unique filename.
			//
			// Prevents:
			// john.png
			// john.png
			// from overwriting each other.
			// ---------------------------------------------
			var fileName =
				$"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";

			var filePath = Path.Combine(
				uploadsFolder,
				fileName);

			// ---------------------------------------------
			// Copy uploaded file to server.
			// ---------------------------------------------
			using (var stream = new FileStream(filePath, FileMode.Create))
			{
				await file.CopyToAsync(stream);
			}

			// ---------------------------------------------
			// Store only the relative URL in database.
			//
			// Good:
			// /uploads/profile-pictures/abc123.jpg
			//
			// Bad:
			// C:\Users\Desktop\Project\...
			// ---------------------------------------------
			employee.ProfilePicture =
				$"/uploads/profile-pictures/{fileName}";

			employee.UpdatedAt = DateTime.UtcNow;

			await _context.SaveChangesAsync();

			return employee.ProfilePicture;
		}
	}
}