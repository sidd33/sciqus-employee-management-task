using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.ComponentModel.DataAnnotations;

namespace EmployeeManagement.BUSINESS.BusinessModels.RequestDTOs.DepartmentRequestDtos
{
	public class UpdateDepartmentDto
	{
		[Required, MaxLength(100)]
		public string Name { get; set; } = string.Empty;

		[MaxLength(500)]
		public string Description { get; set; } = string.Empty;
	}
}