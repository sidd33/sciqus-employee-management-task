using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using EmployeeManagement.DATA.DomainModels.DepartmentDATA;
using EmployeeManagement.DATA.DomainModels.EmployeeDATA;
using Microsoft.EntityFrameworkCore;

namespace EmployeeManagement.DATA.Contexts;

public partial class AppDbContext
{
	public DbSet<Department> Departments { get; set; }
	public DbSet<Employee> Employees { get; set; }

	private void ConfigureDepartmentEmployee(ModelBuilder modelBuilder)
	{
		modelBuilder.Entity<Department>(entity =>
		{
			entity.HasKey(d => d.Id);

			entity.Property(d => d.Name)
				.IsRequired()
				.HasMaxLength(100);

			entity.HasIndex(d => d.Name)
				.IsUnique();

			entity.Property(d => d.Description)
				.HasMaxLength(500);
		});

		modelBuilder.Entity<Employee>(entity =>
		{
			entity.HasKey(e => e.Id);

			entity.Property(e => e.FirstName)
				.IsRequired()
				.HasMaxLength(100);

			entity.Property(e => e.LastName)
				.IsRequired()
				.HasMaxLength(100);

			entity.Property(e => e.Email)
				.IsRequired()
				.HasMaxLength(256);

			entity.HasIndex(e => e.Email)
				.IsUnique();

			entity.Property(e => e.PasswordHash)
				.IsRequired();

			// Deleted employees are excluded from all default queries
			entity.HasQueryFilter(e => !e.IsDeleted);

			entity.HasOne(e => e.Department)
				.WithMany()
				.HasForeignKey(e => e.DepartmentId)
				.OnDelete(DeleteBehavior.Restrict);
		});
	}
}
