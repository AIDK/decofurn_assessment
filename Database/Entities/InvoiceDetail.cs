using CsvHelper.Configuration.Attributes;

namespace assessment.Database.Entities;

public class InvoiceDetail
{
    [Name("LineId")]
    public long Id { get; set; }
    
    [Name("InvoiceNumber")]
    public string InvoiceNumber { get; set; }
    
    [Name("Description")]
    public string Description { get; set; }
    
    [Name("Quantity")]
    public double? Quantity { get; set; }
    
    [Name("UnitSellingPriceExVAT")]
    public double? UnitPrice { get; set; }
}