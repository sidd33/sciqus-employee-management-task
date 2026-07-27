using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EmployeeManagement.DATA.DomainModels.EmployeeDATA;

namespace EmployeeManagement.DATA.DomainModels.DepartmentDATA
{
	public class Department
	{
		public Guid Id { get; set; } = Guid.NewGuid();

		public string Name { get; set; } = string.Empty;

		public string Description { get; set; } = string.Empty;

		public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

		public DateTime? UpdatedAt { get; set; }

		public ICollection<Employee> Employees { get; set; } = new List<Employee>();
		public ICollection<TicketDATA.Ticket> Tickets { get; set; } = new List<TicketDATA.Ticket>();
	}
}