using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeManagement.BUSINESS.BusinessModels.RequestDTOs.EmployeeRequestDtos
{
	public class EmployeeQueryParameters
	{
		public int PageNumber { get; set; } = 1;
		public int PageSize { get; set; } = 10;
		public Guid? DepartmentId { get; set; }
		public string? SearchTerm { get; set; }
	}
}