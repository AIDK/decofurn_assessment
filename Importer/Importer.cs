using assessment.Database;
using assessment.Database.Entities;
using Microsoft.EntityFrameworkCore;

namespace assessment.Importer;

public static class Importer
{
    public static async Task ImportAsync(Context context, List<Invoice> invoices, List<InvoiceDetail> details, CancellationToken cancellationToken = default)
    {
        if (invoices.Count == 0)
        {
            Console.WriteLine("No invoices found");
           return; 
        }
        
        await context.Database.BeginTransactionAsync(cancellationToken);
        
        try
        {
            await ImportInvoiceHeaders(context, invoices, cancellationToken);
            await ImportInvoiceDetails(context, details, cancellationToken);
            
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
        var existingInvoices = 
            context.Invoices.AsNoTracking().ToDictionary(z => z.Number);
            
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

    public static void ValidateImport(Context context)
    {
        //TODO: validate import
        var invoiceLineTotals = context.Lines.Sum(z => z.Quantity * z.UnitPrice) ?? 0;
        var invoiceTotals = context.Invoices.Sum(z => z.Total) ?? 0;

        Console.WriteLine("Data Validation:");
        Console.WriteLine($"Sum of Invoices: {invoiceTotals:N2}");
        Console.WriteLine($"Sum of Invoice Lines: {invoiceLineTotals:N2}");
        Console.WriteLine();
    }

    public static void GenerateSummary(List<InvoiceDetail> details)
    {
        //TODO: summary
        var summary =
            details.GroupBy(line => line.InvoiceNumber)
                .Select(group => new
                {
                    InvoiceNumber = group.Key,
                    TotalQuantity = group.Count()
                }).ToList();

        Console.WriteLine("Summary:");
        foreach (var s in summary)
            Console.WriteLine($"Invoice: {s.InvoiceNumber}, Total: {s.TotalQuantity} ");
    }
}