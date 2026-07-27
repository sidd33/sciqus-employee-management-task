namespace EmployeeManagement.DATA.Contexts;

using Microsoft.EntityFrameworkCore;
using EmployeeManagement.DATA.DomainModels.CustomerDATA;
using EmployeeManagement.DATA.DomainModels.TicketDATA;

public partial class AppDbContext : DbContext
{
    public DbSet<Customer> Customers { get; set; } = null!;
    public DbSet<Ticket> Tickets { get; set; } = null!;
}
