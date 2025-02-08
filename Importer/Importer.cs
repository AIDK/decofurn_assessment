using assessment.Database;
using assessment.Database.Entities;
using Microsoft.EntityFrameworkCore;

namespace assessment.Importer;

public static class Importer
{
    public static async Task ImportAsync(List<Invoice> invoices, List<InvoiceDetail> details, CancellationToken cancellationToken = default)
    {
        if (invoices.Count == 0)
        {
            Console.WriteLine("No invoices found");
           return; 
        }
        
        await using var context = new Context();
        await context.Database.BeginTransactionAsync(cancellationToken);
        
        try
        {
            // uncomment below line to drop DB (for testing purpose only)
            //await context.Database.EnsureDeletedAsync(cancellationToken);
            await context.Database.MigrateAsync(cancellationToken: cancellationToken);
            
            context.Invoices.AddRange(invoices);
            await context.SaveChangesAsync(cancellationToken);
            
            context.Lines.AddRange(details);
            await context.SaveChangesAsync(cancellationToken);
        }
        catch (Exception)
        {
            await context.Database.RollbackTransactionAsync(cancellationToken);
            throw;
        }
    }
}