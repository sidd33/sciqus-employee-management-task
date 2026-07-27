using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using EmployeeManagement.BUSINESS.BusinessModels;
using EmployeeManagement.BUSINESS.BusinessModels.RequestDTOs.DepartmentRequestDtos;
using EmployeeManagement.BUSINESS.BusinessModels.ResponseDTOs.DepartmentResponseDtos;

namespace EmployeeManagement.BUSINESS.Interfaces.IService
{
	public interface IDepartmentService
	{
		Task<PagedResponse<DepartmentResponseDto>> GetAllAsync(DepartmentQueryParameters query);

		Task<DepartmentResponseDto?> GetByIdAsync(Guid id);

		Task<DepartmentResponseDto> CreateAsync(CreateDepartmentDto dto);

		Task<DepartmentResponseDto?> UpdateAsync(Guid id, UpdateDepartmentDto dto);
	}
}
