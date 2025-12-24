using System.ComponentModel.DataAnnotations;
using AiSqlAssistant.Api.Attributes;

namespace AiSqlAssistant.Api.Models;

[TableDescription("Groups products into logical sections like 'Electronics' or 'Clothing'.")]
public class Category
{
    [Key]
    public int Id { get; set; }

    [ColumnDescription("The display name of the category.")]
    public string Name { get; set; } = string.Empty;
}