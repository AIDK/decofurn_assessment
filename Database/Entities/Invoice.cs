using System.ComponentModel.DataAnnotations;
using CsvHelper.Configuration.Attributes;

namespace assessment.Database.Entities;
public class Invoice
{
    [Key]
    [Name("InvoiceId")]
    public long Id { get; set; }
    
    [Name("InvoiceNumber")]
    public string Number { get; set; }
    
    [Name("InvoiceDate"), DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
    public DateTime? Date { get; set; }
    
    [Name("Address")]
    public string Address { get; set; }
    
    [Name("InvoiceTotal")]
    public double? Total { get; set; }
}