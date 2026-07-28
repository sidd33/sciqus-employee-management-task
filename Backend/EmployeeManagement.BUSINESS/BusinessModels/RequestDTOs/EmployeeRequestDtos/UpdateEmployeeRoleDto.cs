using System.ComponentModel.DataAnnotations;
using EmployeeManagement.DATA.DomainModels.EmployeeDATA;

namespace EmployeeManagement.BUSINESS.BusinessModels.RequestDTOs.EmployeeRequestDtos
{
	public class UpdateEmployeeRoleDto
	{
		[Required]
		public EmployeeRole Role { get; set; }
	}
}
