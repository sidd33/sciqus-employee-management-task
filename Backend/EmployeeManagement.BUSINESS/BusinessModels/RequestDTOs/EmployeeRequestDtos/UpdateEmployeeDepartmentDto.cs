using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.ComponentModel.DataAnnotations;

namespace EmployeeManagement.BUSINESS.BusinessModels.RequestDTOs.EmployeeRequestDtos
{
	public class UpdateEmployeeDepartmentDto
	{
		[Required]
		public Guid DepartmentId { get; set; }
	}
}
