using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.ComponentModel.DataAnnotations;
using EmployeeManagement.DATA.DomainModels.EmployeeDATA;

namespace EmployeeManagement.BUSINESS.BusinessModels.RequestDTOs.EmployeeRequestDtos
{
	public class CreateEmployeeDto
	{
		[Required, MaxLength(50)]
		public string FirstName { get; set; } = string.Empty;

		[Required, MaxLength(50)]
		public string LastName { get; set; } = string.Empty;

		[Required, EmailAddress]
		public string Email { get; set; } = string.Empty;

		[Required, MinLength(6)]
		public string Password { get; set; } = string.Empty;

		[Required]
		public Guid DepartmentId { get; set; }

		public EmployeeRole Role { get; set; } = EmployeeRole.Employee;
	}
}
