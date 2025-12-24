using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using AiSqlAssistant.Api.Attributes;

namespace AiSqlAssistant.Api.Models;

[TableDescription("The main inventory table containing items for sale.")]
public class Product
{
    [Key]
    public int Id { get; set; }

    [ColumnDescription("The commercial name of the product.")]
    public string Name { get; set; } = string.Empty;

    [ColumnDescription("The unit price in USD.")]
    public decimal Price { get; set; }

    [ColumnDescription("Current stock level. 0 means out of stock.")]
    public int Stock { get; set; }

    // Foreign Key
    public int CategoryId { get; set; }
    
    [ForeignKey(nameof(CategoryId))]
    public Category? Category { get; set; }
}