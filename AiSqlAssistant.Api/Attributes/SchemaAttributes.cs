namespace AiSqlAssistant.Api.Attributes;

[AttributeUsage(AttributeTargets.Class)]
public class TableDescriptionAttribute(string description) : Attribute
{
    public string Description { get; } = description;
}

[AttributeUsage(AttributeTargets.Property)]
public class ColumnDescriptionAttribute(string description) : Attribute
{
    public string Description { get; } = description;
}