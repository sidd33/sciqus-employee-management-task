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
					Name = "IT Support",
					Description = "Handles technical and infrastructure issues.",
					CreatedAt = DateTime.UtcNow
				},
				new Department
				{
					Id = Guid.NewGuid(),
					Name = "Billing",
					Description = "Handles invoices, payments, and refunds.",
					CreatedAt = DateTime.UtcNow
				},
				new Department
				{
					Id = Guid.NewGuid(),
					Name = "General Inquiries",
					Description = "Handles miscellaneous customer questions.",
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
				Role = EmployeeRole.Admin,
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
				new DomainModels.CustomerDATA.Customer
				{
					Id = Guid.NewGuid(),
					Name = "John Doe",
					Email = "john.doe@example.com",
					Password = BCrypt.Net.BCrypt.HashPassword("Customer@123")
				},
				new DomainModels.CustomerDATA.Customer
				{
					Id = Guid.NewGuid(),
					Name = "Jane Smith",
					Email = "jane.smith@example.com",
					Password = BCrypt.Net.BCrypt.HashPassword("Customer@123")
				}
			};

			await context.Customers.AddRangeAsync(customers);
			await context.SaveChangesAsync();
		}

		private static async Task SeedTicketsAsync(AppDbContext context)
		{
			if (await context.Tickets.AnyAsync()) return;

			var customer = await context.Customers.FirstAsync();
			var itDepartment = await context.Departments.FirstOrDefaultAsync(d => d.Name == "IT Support");
			var billingDepartment = await context.Departments.FirstOrDefaultAsync(d => d.Name == "Billing");

			var itEmployee = await context.Employees.FirstOrDefaultAsync(e => e.DepartmentId == itDepartment!.Id && e.Role != EmployeeRole.Admin);
			var billingEmployee = await context.Employees.FirstOrDefaultAsync(e => e.DepartmentId == billingDepartment!.Id && e.Role != EmployeeRole.Admin);

			if (customer == null || itDepartment == null || itEmployee == null) return;

			var tickets = new[]
			{
				new DomainModels.TicketDATA.Ticket
				{
					Id = Guid.NewGuid(),
					Title = "VPN Connection Issue",
					Description = "Cannot connect to company VPN from home.",
					CustomerId = customer.Id,
					DepartmentId = itDepartment.Id,
					AssignedEmployeeId = itEmployee.Id,
					Status = DomainModels.TicketDATA.TicketStatus.Assigned,
					CreatedAt = DateTime.UtcNow,
					SlaStartTime = DateTime.UtcNow.AddHours(4)
				},
				new DomainModels.TicketDATA.Ticket
				{
					Id = Guid.NewGuid(),
					Title = "Invoice Refund Query",
					Description = "Requesting clarification on double billing invoice #1042.",
					CustomerId = customer.Id,
					DepartmentId = billingDepartment!.Id,
					AssignedEmployeeId = billingEmployee!.Id,
					Status = DomainModels.TicketDATA.TicketStatus.InProgress,
					CreatedAt = DateTime.UtcNow,
					SlaStartTime = DateTime.UtcNow.AddHours(4)
				}
			};

			await context.Tickets.AddRangeAsync(tickets);
			await context.SaveChangesAsync();
		}
	}
}
