using System;
using System.Collections.Generic;
using EmployeeManagement.DATA.DomainModels.DepartmentDATA;

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


		// Auth/Ticket relationship
		public ICollection<TicketDATA.Ticket> AssignedTickets { get; set; }
			= new List<TicketDATA.Ticket>();


		// Round-robin ticket assignment tracking
		// Null means employee has never received a ticket
		public DateTime? LastAssignedTicketAt { get; set; }
	}
}