using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using EmployeeManagement.DATA.DomainModels.EmployeeDATA;

namespace EmployeeManagement.BUSINESS.BusinessModels.ResponseDTOs.EmployeeResponseDtos
{
	public class EmployeeResponseDto
	{
		public Guid Id { get; set; }
		public string FirstName { get; set; } = string.Empty;
		public string LastName { get; set; } = string.Empty;
		public string Email { get; set; } = string.Empty;
		public DateTime JoinedAt { get; set; }
		public string? ProfilePicture { get; set; }
		public EmployeeRole Role { get; set; }
		public bool IsActive { get; set; }
		public DateTime UpdatedAt { get; set; }
		public Guid DepartmentId { get; set; }
		public string DepartmentName { get; set; } = string.Empty;
	}
}
