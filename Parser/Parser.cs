using System.Globalization;
using assessment.Database.Entities;
using CsvHelper;
using CsvHelper.Configuration;

namespace assessment.Parser;

public static class Parser
{
    private static readonly CsvConfiguration Configuration = 
        new(CultureInfo.InvariantCulture)
        {
            HasHeaderRecord = true,
            MissingFieldFound = null
        };
    
    public static List<Invoice> ParseHeader(string filePath)
    {
        try
        {
            using var sr = new StreamReader(filePath);
            using var csv = new CsvReader(sr, Configuration);
        
            var invoices = new List<Invoice>();
            csv.Read();
            csv.ReadHeader();

            while (csv.Read())
            {
                var invoice = new Invoice
                {
                    Number = csv.GetField<string>("Invoice Number")!,
                    Address = csv.GetField<string>("Address")!,
                    Total = csv.GetField<double>("Invoice Total Ex VAT"),
                    
                    // attempt to parse csv date
                    Date = ParseDate(csv.GetField<string>("Invoice Date"))
                };

                invoices.Add(invoice);
            }
                
            // TODO: replace this with better solution
            return invoices
                .GroupBy(z => z.Number)
                .Select(z => z.First())
                .ToList();
        }
        catch (Exception e)
        {
            Console.WriteLine($"exception occurred parsing the invoice headers in the csv file: {e.Message}");
            throw;
        }
    }

    public static List<InvoiceDetail> ParseDetail(string filePath)
    {
        try
        {
            using var sr = new StreamReader(filePath);
            using var csv = new CsvReader(sr, Configuration);
        
            var details = new List<InvoiceDetail>();
            csv.Read();
            csv.ReadHeader();
        
            while (csv.Read())
            {
                var detail = new InvoiceDetail
                {
                    InvoiceNumber = csv.GetField<string>("Invoice Number")!,
                    Description = csv.GetField<string>("Line description")!,
                    Quantity = csv.GetField<double>("Invoice Quantity"),
                    UnitPrice = csv.GetField<double>("Unit selling price ex VAT")
                };
            
                details.Add(detail);
            }
        
            return details;
        }
        catch (Exception e)
        {
            Console.WriteLine($"exception occurred parsing the invoice details in the csv file: {e.Message}");
            throw;
        }
    }

    private static DateTime? ParseDate(string? dateString)
    {
        if (string.IsNullOrWhiteSpace(dateString)) return null;
        
        if (DateTime.TryParseExact(dateString, "dd/MM/yyyy HH:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out var dateParsed))
            return dateParsed;

        if (DateTime.TryParse(dateString, out var parsedDate))
            return parsedDate;
        
        Console.WriteLine($"Unable to parse date: {dateString}");
        return null;
        
    }
}