using assessment.Database.Entities;
using Microsoft.EntityFrameworkCore;

namespace assessment.Database;

public class Context : DbContext
{
    public DbSet<Invoice> Invoices { get; set; }
    public DbSet<InvoiceDetail> Lines { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder options) =>
        options.UseSqlServer("<connection string>");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Invoice>().ToTable("Invoice").Property(z => z.Number).IsRequired();
        modelBuilder.Entity<InvoiceDetail>().ToTable("InvoiceDetail").Property(z => z.InvoiceNumber).IsRequired();
    }
}