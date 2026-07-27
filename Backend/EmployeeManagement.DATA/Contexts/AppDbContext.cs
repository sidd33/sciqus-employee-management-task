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
            
            // Call partial methods here if needed
            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
