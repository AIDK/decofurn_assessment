using assessment.Database;
using DataImport = assessment.Importer.Importer;

namespace assessment;

internal static class Program
{
    private static async Task Main(string[] _)
    {
        var cts = new CancellationTokenSource();
        
        
        try
        {
            const string filePath = "<path-to-file>";
            if (!File.Exists(filePath))
            {
                Console.WriteLine("File not found");
                return;
            }
            
            var invoices = Parser.Parser.ParseHeader(filePath);
            var lines = Parser.Parser.ParseDetail(filePath);
            
            await using var context = new Context();
            
            await context.Database.EnsureCreatedAsync(cts.Token);
            await DataImport.ImportAsync(context, invoices, lines, cts.Token);
            
            DataImport.ValidateImport(context);
            DataImport.GenerateSummary(lines);
        }
        catch (Exception e)
        {
            Console.WriteLine($"exception occurred: {e}");
        }
        finally
        {
            cts.Dispose();
        }
    }
}