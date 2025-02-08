using assessment.Database.Entities;
using Microsoft.EntityFrameworkCore;

namespace assessment.Database;

public class Context : DbContext
{
    public DbSet<Invoice> Invoices { get; set; }
    public DbSet<InvoiceDetail> Lines { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder options) =>
        options.UseSqlServer(@"Server=localhost\SQLEXPRESS;Database=df;User Id=sa;Password=123456;Trusted_Connection=True;TrustServerCertificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        //
    }
}