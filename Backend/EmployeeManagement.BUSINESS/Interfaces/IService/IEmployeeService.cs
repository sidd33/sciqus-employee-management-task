using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using EmployeeManagement.BUSINESS.BusinessModels;
using EmployeeManagement.BUSINESS.BusinessModels.RequestDTOs.EmployeeRequestDtos;
using EmployeeManagement.BUSINESS.BusinessModels.ResponseDTOs.EmployeeResponseDtos;
using Microsoft.AspNetCore.Http;

namespace EmployeeManagement.BUSINESS.Interfaces.IService
{
	public interface IEmployeeService
	{
		Task<PagedResponse<EmployeeResponseDto>> GetAllAsync(EmployeeQueryParameters query);

		Task<EmployeeResponseDto?> GetByIdAsync(Guid id);

		Task<EmployeeResponseDto> CreateAsync(CreateEmployeeDto dto);

		Task<EmployeeResponseDto?> UpdateAsync(Guid id, UpdateEmployeeDto dto);

		Task<EmployeeResponseDto?> UpdateDepartmentAsync(Guid id, UpdateEmployeeDepartmentDto dto);

		Task<EmployeeResponseDto?> UpdateRoleAsync(Guid id, UpdateEmployeeRoleDto dto, string currentUserRole);

		// Consider renaming to FireEmployeeAsync if you want the interface
		// to match the business requirement.
		Task<bool> SoftDeleteAsync(Guid id);

		Task<string?> UploadProfilePictureAsync(Guid id, IFormFile file);
	}
}
