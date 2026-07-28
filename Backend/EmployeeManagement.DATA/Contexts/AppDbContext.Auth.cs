using EmployeeManagement.DATA.DomainModels.AuthDATA;
using Microsoft.EntityFrameworkCore;

namespace EmployeeManagement.DATA.Contexts
{
    public partial class AppDbContext
    {
        public DbSet<RefreshToken> RefreshTokens { get; set; }

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
