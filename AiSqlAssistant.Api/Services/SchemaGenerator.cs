using System.Reflection;
using System.Text;
using AiSqlAssistant.Api.Attributes;
using AiSqlAssistant.Api.Data;
using Microsoft.EntityFrameworkCore;

namespace AiSqlAssistant.Api.Services;

public class SchemaGenerator(AppDbContext dbContext)
{
    public string GetSchemaDefinition()
    {
        var sb = new StringBuilder();
        var entityTypes = dbContext.Model.GetEntityTypes();

        foreach (var entityType in entityTypes)
        {
            var clrType = entityType.ClrType;
            var tableAttr = clrType.GetCustomAttribute<TableDescriptionAttribute>();
            string tableDesc = tableAttr != null ? $" ({tableAttr.Description})" : "";

            sb.AppendLine($"Table: {entityType.GetTableName()}{tableDesc}");
            sb.AppendLine("Columns:");

            foreach (var property in entityType.GetProperties())
            {
                var propInfo = property.PropertyInfo;
                if (propInfo == null) continue;

                var colAttr = propInfo.GetCustomAttribute<ColumnDescriptionAttribute>();
                string colDesc = colAttr != null ? $" -- {colAttr.Description}" : "";
                
                // Get SQL-friendly type name
                string typeName = GetFriendlyTypeName(propInfo.PropertyType);
                
                // Identify Keys
                string keyInfo = property.IsPrimaryKey() ? "[PK]" : "";
                if (property.IsForeignKey())
                {
                    var fk = property.GetContainingForeignKeys().First();
                    keyInfo = $"[FK -> {fk.PrincipalEntityType.GetTableName()}.{fk.PrincipalKey.Properties.First().Name}]";
                }

                sb.AppendLine($"  - {property.Name} ({typeName}) {keyInfo}{colDesc}");
            }
            sb.AppendLine();
        }
        return sb.ToString();
    }

    private string GetFriendlyTypeName(Type type)
    {
        if (type == typeof(int) || type == typeof(long)) return "INTEGER";
        if (type == typeof(decimal) || type == typeof(double)) return "NUMBER";
        if (type == typeof(bool)) return "BOOLEAN";
        if (type == typeof(DateTime)) return "DATETIME";
        return "TEXT";
    }
}