using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EmployeeManagement.DATA.DomainModels.DepartmentDATA;
using EmployeeManagement.DATA.DomainModels.EmployeeDATA;



namespace EmployeeManagement.DATA.DomainModels.EmployeeDATA
{
	public class Employee
	{
		public Guid Id { get; set; }

		public string FirstName { get; set; } = string.Empty;

		public string LastName { get; set; } = string.Empty;

		public string Email { get; set; } = string.Empty;

		public string PasswordHash { get; set; } = string.Empty;

		public DateTime JoinedAt { get; set; }

		public string? ProfilePicture { get; set; }

		public EmployeeRole Role { get; set; }

		public bool IsActive { get; set; } = true;

		public DateTime UpdatedAt { get; set; }

		public DateTime? DeletedDate { get; set; }

		public bool IsDeleted { get; set; } = false;

		// Foreign key
		public Guid DepartmentId { get; set; }

		// Navigation property
		public Department Department { get; set; } = null!;
		
		public ICollection<TicketDATA.Ticket> AssignedTickets { get; set; } = new List<TicketDATA.Ticket>();
	}
}