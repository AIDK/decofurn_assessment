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

            // TODO: import invoice header
            await ImportInvoiceHeaders(context, invoices, cancellationToken);
            
            // TODO: import invoice details
            await ImportInvoiceDetails(context, details, cancellationToken);
            
            // TODO: validate imported data
            ValidateImport(context);
            
            // TODO: generate import summary
            GenerateSummary(details);

            await context.Database.CommitTransactionAsync(cancellationToken);
        }
        catch (Exception)
        {
            await context.Database.RollbackTransactionAsync(cancellationToken);
            throw;
        }
    }

    private static async Task ImportInvoiceHeaders(Context context, List<Invoice> invoices, CancellationToken cancellationToken)
    {
        //TODO: load invoices into memory to check for duplicates
        var existingInvoices = 
            context.Invoices.AsNoTracking().ToDictionary(z => z.Number);
            
        //TODO: add new invoices
        foreach (var invoice in invoices.Where(invoice => !existingInvoices.ContainsKey(invoice.Number)))
            context.Invoices.AddRange(invoice);

        await context.SaveChangesAsync(cancellationToken);
        
    }

    private static async Task ImportInvoiceDetails(Context context, List<InvoiceDetail> details,
        CancellationToken cancellationToken)
    {
        var existingLines = context.Lines.AsNoTracking()
            .GroupBy(z => z.InvoiceNumber)
            .ToDictionary(z => z.Key, z => z.ToList());

        foreach (var line in details)
        {
            if (!existingLines.TryGetValue(line.InvoiceNumber, out var existingLine))
            {
                // there are no existing lines
                context.Lines.Add(line);
                continue;
            }

            var isDuplicate = existingLine.Any(
                e => e.Description == line.Description && 
                     e.Quantity - line.Quantity < 0.001 && 
                     e.UnitPrice - line.UnitPrice < 0.001);
            
            if (!isDuplicate)
                context.Lines.AddRange(line);
        }
        
        await context.SaveChangesAsync(cancellationToken);
    }

    private static void ValidateImport(Context context)
    {
        var invoiceLineTotals = context.Lines.Sum(z => z.Quantity * z.UnitPrice) ?? 0;
        var invoiceTotals = context.Invoices.Sum(z => z.Total) ?? 0;

        Console.WriteLine("\nData Validation:");
        Console.WriteLine($"Sum of Invoices: {invoiceTotals:N2}");
        Console.WriteLine($"Sum of Invoice Lines: {invoiceLineTotals:N2}");

        //NOTE: because of the doubles we have to compare perform some kind of comparison and allow 
        // for tolerances (might not work but worth a try)
        Console.WriteLine(Math.Abs(invoiceLineTotals - invoiceTotals) < 0.01 ? "Validation Success" : "Oops...");
    }

    private static void GenerateSummary(List<InvoiceDetail> details)
    {
        //TODO: summary
        var summary =
            details.GroupBy(line => line.InvoiceNumber)
                .Select(group => new
                {
                    InvoiceNumber = group.Key,
                    TotalQuantity = group.Sum(line => line.Quantity)
                }).ToList();

        Console.WriteLine("\nSummary:");
        foreach (var s in summary)
            Console.WriteLine($"Invoice: {s.InvoiceNumber}, Total: {s.TotalQuantity} ");
    }
}