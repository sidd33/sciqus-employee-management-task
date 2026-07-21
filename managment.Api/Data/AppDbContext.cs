using managment.Api.Entities;
using Microsoft.EntityFrameworkCore;

namespace managment.Api.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Employee> Employees => Set<Employee>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Employee>().ToTable("Employees");

        modelBuilder.Entity<Employee>()
            .Property(e => e.Role)
            .HasConversion<int>();
    }
}
