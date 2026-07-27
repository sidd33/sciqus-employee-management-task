using EmployeeManagement.DATA.DomainModels.AuthDATA;
using Microsoft.EntityFrameworkCore;

namespace EmployeeManagement.DATA.Contexts
{
    // Sidd's partial class for Auth-related DbSets
    public partial class AppDbContext
    {
        public DbSet<RefreshToken> RefreshTokens { get; set; }

        // Any specific model configuration for Auth can go here
        partial void OnModelCreatingPartial(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<RefreshToken>()
                .HasKey(rt => rt.Id);
                
            modelBuilder.Entity<RefreshToken>()
                .Property(rt => rt.Token)
                .IsRequired();
        }
    }
}
