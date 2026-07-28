using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using EmployeeManagement.DATA.Contexts;
using EmployeeManagement.DATA.DomainModels.DepartmentDATA;
using EmployeeManagement.DATA.DomainModels.EmployeeDATA;
using Microsoft.EntityFrameworkCore;

namespace EmployeeManagement.DATA.Seed
{
	public static class DbInitializer
	{
		public static async Task SeedAsync(AppDbContext context)
		{
			await context.Database.MigrateAsync();

			await SeedDepartmentsAsync(context);
			await SeedEmployeesAsync(context);
			await SeedCustomersAsync(context);
			await SeedTicketsAsync(context);
		}

		private static async Task SeedDepartmentsAsync(AppDbContext context)
		{
			if (await context.Departments.AnyAsync())
				return;

			var departments = new[]
			{
				new Department
				{
					Id = Guid.NewGuid(),
					Name = "HR",
					Description = "Human Resources and Employee Relations.",
					CreatedAt = DateTime.UtcNow
				},
				new Department
				{
					Id = Guid.NewGuid(),
					Name = "Software",
					Description = "Development.",
					CreatedAt = DateTime.UtcNow
				},
				new Department
				{
					Id = Guid.NewGuid(),
					Name = "Sales",
					Description = "Client acquisition and sales.",
					CreatedAt = DateTime.UtcNow
				},
				new Department
				{
					Id = Guid.NewGuid(),
					Name = "Advertising",
					Description = "Marketing.",
					CreatedAt = DateTime.UtcNow
				}
			};

			await context.Departments.AddRangeAsync(departments);
			await context.SaveChangesAsync();
		}

		private static async Task SeedEmployeesAsync(AppDbContext context)
		{
			if (await context.Employees.AnyAsync())
				return;

			var admin = new Employee
			{
				Id = Guid.NewGuid(),
				FirstName = "System",
				LastName = "Admin",
				Email = "admin@company.com",
				PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin@123"),
				JoinedAt = DateTime.UtcNow,
				Role = EmployeeRole.SuperAdmin,
				IsActive = true,
				IsDeleted = false,
				UpdatedAt = DateTime.UtcNow,
				DepartmentId = await context.Departments
					.OrderBy(d => d.Name)
					.Select(d => d.Id)
					.FirstAsync()
			};

			await context.Employees.AddAsync(admin);

			var departments = await context.Departments.ToListAsync();

			int counter = 1;

			foreach (var department in departments)
			{
				for (int i = 1; i <= 2; i++)
				{
					var employee = new Employee
					{
						Id = Guid.NewGuid(),
						FirstName = $"Employee{counter}",
						LastName = department.Name.Replace(" ", ""),
						Email = $"employee{counter}@company.com",
						PasswordHash = BCrypt.Net.BCrypt.HashPassword("Employee@123"),
						JoinedAt = DateTime.UtcNow,
						Role = EmployeeRole.Employee,
						IsActive = true,
						IsDeleted = false,
						UpdatedAt = DateTime.UtcNow,
						DepartmentId = department.Id,
						LastAssignedTicketAt = null
					};

					await context.Employees.AddAsync(employee);
					counter++;
				}
			}

			await context.SaveChangesAsync();
		}

		private static async Task SeedCustomersAsync(AppDbContext context)
		{
			if (await context.Customers.AnyAsync()) return;

			var customers = new[]
			{
				new DomainModels.CustomerDATA.Customer { Id = Guid.NewGuid(), Name = "Alice Johnson", Email = "alice.johnson@example.com", Password = BCrypt.Net.BCrypt.HashPassword("Customer@123") },
				new DomainModels.CustomerDATA.Customer { Id = Guid.NewGuid(), Name = "Bob Smith", Email = "bob.smith@example.com", Password = BCrypt.Net.BCrypt.HashPassword("Customer@123") },
				new DomainModels.CustomerDATA.Customer { Id = Guid.NewGuid(), Name = "Charlie Davis", Email = "charlie.davis@example.com", Password = BCrypt.Net.BCrypt.HashPassword("Customer@123") },
				new DomainModels.CustomerDATA.Customer { Id = Guid.NewGuid(), Name = "Diana Prince", Email = "diana.prince@example.com", Password = BCrypt.Net.BCrypt.HashPassword("Customer@123") },
				new DomainModels.CustomerDATA.Customer { Id = Guid.NewGuid(), Name = "Ethan Hunt", Email = "ethan.hunt@example.com", Password = BCrypt.Net.BCrypt.HashPassword("Customer@123") },
				new DomainModels.CustomerDATA.Customer { Id = Guid.NewGuid(), Name = "Fiona Gallagher", Email = "fiona.gallagher@example.com", Password = BCrypt.Net.BCrypt.HashPassword("Customer@123") },
				new DomainModels.CustomerDATA.Customer { Id = Guid.NewGuid(), Name = "George Clark", Email = "george.clark@example.com", Password = BCrypt.Net.BCrypt.HashPassword("Customer@123") },
				new DomainModels.CustomerDATA.Customer { Id = Guid.NewGuid(), Name = "Hannah Abbott", Email = "hannah.abbott@example.com", Password = BCrypt.Net.BCrypt.HashPassword("Customer@123") },
				new DomainModels.CustomerDATA.Customer { Id = Guid.NewGuid(), Name = "Ian Malcolm", Email = "ian.malcolm@example.com", Password = BCrypt.Net.BCrypt.HashPassword("Customer@123") },
				new DomainModels.CustomerDATA.Customer { Id = Guid.NewGuid(), Name = "Julia Roberts", Email = "julia.roberts@example.com", Password = BCrypt.Net.BCrypt.HashPassword("Customer@123") }
			};

			await context.Customers.AddRangeAsync(customers);
			await context.SaveChangesAsync();
		}

		private static async Task SeedTicketsAsync(AppDbContext context)
		{
			if (await context.Tickets.AnyAsync()) return;

			var customers = await context.Customers.ToListAsync();
			var hrDept = await context.Departments.FirstOrDefaultAsync(d => d.Name == "HR");
			var seDept = await context.Departments.FirstOrDefaultAsync(d => d.Name == "Software Engineering");
			var salesDept = await context.Departments.FirstOrDefaultAsync(d => d.Name == "Sales");

			var hrEmp = await context.Employees.FirstOrDefaultAsync(e => e.DepartmentId == hrDept!.Id && e.Role != EmployeeRole.Admin);
			var seEmp = await context.Employees.FirstOrDefaultAsync(e => e.DepartmentId == seDept!.Id && e.Role != EmployeeRole.Admin);
			var salesEmp = await context.Employees.FirstOrDefaultAsync(e => e.DepartmentId == salesDept!.Id && e.Role != EmployeeRole.Admin);

			var tickets = new[]
			{
				new DomainModels.TicketDATA.Ticket { Id = Guid.NewGuid(), Title = "VPN Connection Drop", Description = "Cannot access corporate VPN from home network.", CustomerId = customers[0].Id, DepartmentId = seDept!.Id, AssignedEmployeeId = seEmp!.Id, Status = DomainModels.TicketDATA.TicketStatus.Assigned, CreatedAt = DateTime.UtcNow.AddDays(-10), SlaStartTime = DateTime.UtcNow.AddDays(-10).AddHours(4) },
				new DomainModels.TicketDATA.Ticket { Id = Guid.NewGuid(), Title = "Double Billing Charge", Description = "Billed twice for subscription invoice #9921.", CustomerId = customers[1].Id, DepartmentId = salesDept!.Id, AssignedEmployeeId = salesEmp!.Id, Status = DomainModels.TicketDATA.TicketStatus.InProgress, CreatedAt = DateTime.UtcNow.AddDays(-9), SlaStartTime = DateTime.UtcNow.AddDays(-9).AddHours(4) },
				new DomainModels.TicketDATA.Ticket { Id = Guid.NewGuid(), Title = "Account Upgrade Query", Description = "Inquiring about Enterprise plan tier pricing.", CustomerId = customers[2].Id, DepartmentId = hrDept!.Id, AssignedEmployeeId = hrEmp!.Id, Status = DomainModels.TicketDATA.TicketStatus.Completed, CreatedAt = DateTime.UtcNow.AddDays(-8), SlaStartTime = DateTime.UtcNow.AddDays(-8).AddHours(4) },
				new DomainModels.TicketDATA.Ticket { Id = Guid.NewGuid(), Title = "Laptop Keyboard Faulty", Description = "Spacebar key is unresponsive on company hardware.", CustomerId = customers[3].Id, DepartmentId = seDept.Id, AssignedEmployeeId = seEmp.Id, Status = DomainModels.TicketDATA.TicketStatus.Assigned, CreatedAt = DateTime.UtcNow.AddDays(-7), SlaStartTime = DateTime.UtcNow.AddDays(-7).AddHours(4) },
				new DomainModels.TicketDATA.Ticket { Id = Guid.NewGuid(), Title = "Refund Delay Notice", Description = "Refund requested 5 days ago has not settled yet.", CustomerId = customers[4].Id, DepartmentId = salesDept.Id, AssignedEmployeeId = salesEmp.Id, Status = DomainModels.TicketDATA.TicketStatus.InProgress, CreatedAt = DateTime.UtcNow.AddDays(-6), SlaStartTime = DateTime.UtcNow.AddDays(-6).AddHours(4) },
				new DomainModels.TicketDATA.Ticket { Id = Guid.NewGuid(), Title = "Office Hours Question", Description = "What are the holiday operating support hours?", CustomerId = customers[5].Id, DepartmentId = hrDept.Id, AssignedEmployeeId = hrEmp.Id, Status = DomainModels.TicketDATA.TicketStatus.Closed, CreatedAt = DateTime.UtcNow.AddDays(-5), SlaStartTime = DateTime.UtcNow.AddDays(-5).AddHours(4) },
				new DomainModels.TicketDATA.Ticket { Id = Guid.NewGuid(), Title = "Password Reset Token", Description = "Password reset link expired immediately.", CustomerId = customers[6].Id, DepartmentId = seDept.Id, AssignedEmployeeId = seEmp.Id, Status = DomainModels.TicketDATA.TicketStatus.Reopened, CreatedAt = DateTime.UtcNow.AddDays(-4), SlaStartTime = DateTime.UtcNow.AddDays(-4).AddHours(4) },
				new DomainModels.TicketDATA.Ticket { Id = Guid.NewGuid(), Title = "Tax Invoice Receipt", Description = "Need official tax invoice for Q3 tax filing.", CustomerId = customers[7].Id, DepartmentId = salesDept.Id, AssignedEmployeeId = salesEmp.Id, Status = DomainModels.TicketDATA.TicketStatus.Completed, CreatedAt = DateTime.UtcNow.AddDays(-3), SlaStartTime = DateTime.UtcNow.AddDays(-3).AddHours(4) },
				new DomainModels.TicketDATA.Ticket { Id = Guid.NewGuid(), Title = "Email Alias Setup", Description = "Requesting new sales team email forwarder alias.", CustomerId = customers[8].Id, DepartmentId = seDept.Id, AssignedEmployeeId = seEmp.Id, Status = DomainModels.TicketDATA.TicketStatus.Assigned, CreatedAt = DateTime.UtcNow.AddDays(-2), SlaStartTime = DateTime.UtcNow.AddDays(-2).AddHours(4) },
				new DomainModels.TicketDATA.Ticket { Id = Guid.NewGuid(), Title = "Partner Portal Info", Description = "Seeking documentation on B2B API integrations.", CustomerId = customers[9].Id, DepartmentId = hrDept.Id, AssignedEmployeeId = hrEmp.Id, Status = DomainModels.TicketDATA.TicketStatus.InProgress, CreatedAt = DateTime.UtcNow.AddDays(-1), SlaStartTime = DateTime.UtcNow.AddDays(-1).AddHours(4) }
			};

			await context.Tickets.AddRangeAsync(tickets);
			await context.SaveChangesAsync();
		}
	}
}
