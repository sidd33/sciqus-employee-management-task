using Microsoft.EntityFrameworkCore;

namespace EmployeeManagement.DATA.Contexts
{
    // This is the base DbContext. Other developers will add their DbSets in partial classes.
    public partial class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            
            // Prevent SQL Server multiple cascade path errors
            modelBuilder.Entity<EmployeeManagement.DATA.DomainModels.TicketDATA.Ticket>()
                .HasOne(t => t.AssignedEmployee)
                .WithMany(e => e.AssignedTickets)
                .HasForeignKey(t => t.AssignedEmployeeId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<EmployeeManagement.DATA.DomainModels.TicketDATA.Ticket>()
                .HasOne(t => t.Department)
                .WithMany(d => d.Tickets)
                .HasForeignKey(t => t.DepartmentId)
                .OnDelete(DeleteBehavior.Restrict);

            // Call partial methods here if needed
            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
