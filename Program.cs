using DataImport = assessment.Importer.Importer;

namespace assessment;

internal static class Program
{
    private static async Task Main(string[] _)
    {
        var cts = new CancellationTokenSource();
        
        try
        {
            const string filePath = @"E:\Projects\Personal\Assessments\decofurn\assessment\Files\data.csv";
            if (!File.Exists(filePath))
            {
                Console.WriteLine("File not found");
                return;
            }
            
            var invoices = Parser.Parser.ParseHeader(filePath);
            var lines = Parser.Parser.ParseDetail(filePath);
            
            await DataImport.ImportAsync(invoices, lines, cts.Token);
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