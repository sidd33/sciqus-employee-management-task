using System;
using System.Collections.Generic;


using System;
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
		// =====================================================
		// SEED DATABASE
		//
		// Called once at startup (from Program.cs) after
		// applying migrations. Safe to call every run —
		// each block checks whether data already exists
		// before inserting anything.
		// =====================================================
		public static async Task SeedAsync(AppDbContext context)
		{
			// Make sure all pending migrations are applied
			// before seeding. Safe no-op if already up to date.
			await context.Database.MigrateAsync();

			await SeedDepartmentsAsync(context);
			await SeedEmployeesAsync(context);
		}

		// =====================================================
		// SEED DEPARTMENTS
		// =====================================================
		private static async Task SeedDepartmentsAsync(AppDbContext context)
		{
			// Skip if departments already exist.
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

		// =====================================================
		// SEED EMPLOYEES
		//
		// Creates:
		// 1. One Admin account.
		// 2. Two Employee accounts per department (for testing
		//    round robin ticket assignment later).
		//
		// NOTE: Passwords are hashed with BCrypt, same as
		// EmployeeService.CreateAsync does at runtime.
		// =====================================================
		private static async Task SeedEmployeesAsync(AppDbContext context)
		{
			// Skip if employees already exist.
			if (await context.Employees.AnyAsync())
				return;

			// ---------------------------------------------
			// Admin account
			//
			// Login: admin@company.com / Admin@123
			// Change this password after first login in a
			// real deployment — this is a dev/test seed only.
			// ---------------------------------------------
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
				// Admin still needs a department FK — assign to the first one.
				DepartmentId = await context.Departments
					.OrderBy(d => d.Name)
					.Select(d => d.Id)
					.FirstAsync()
			};

			await context.Employees.AddAsync(admin);

			// ---------------------------------------------
			// Two regular employees per department.
			//
			// Login password for all seeded employees:
			// Employee@123
			// ---------------------------------------------
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
						// Left null intentionally — round robin picks
						// employees with the oldest/null LastAssignedTicketAt first.
						LastAssignedTicketAt = null
					};

					await context.Employees.AddAsync(employee);
					counter++;
				}
			}

			await context.SaveChangesAsync();
		}
	}
}
