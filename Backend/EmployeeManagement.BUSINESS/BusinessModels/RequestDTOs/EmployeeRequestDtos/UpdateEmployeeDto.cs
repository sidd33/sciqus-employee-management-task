using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace EmployeeManagement.BUSINESS.BusinessModels.RequestDTOs.EmployeeRequestDtos
{
	public class UpdateEmployeeDto
	{
		[Required, MaxLength(50)]
		public string FirstName { get; set; } = string.Empty;

		[Required, MaxLength(50)]
		public string LastName { get; set; } = string.Empty;

		[Required, EmailAddress]
		public string Email { get; set; } = string.Empty;

		public bool IsActive { get; set; } = true;
	}
}
