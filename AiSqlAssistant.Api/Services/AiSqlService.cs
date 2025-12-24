using Dapper;
using Microsoft.Data.Sqlite;
using Microsoft.SemanticKernel.ChatCompletion;

namespace AiSqlAssistant.Api.Services;

public class AiSqlService(
    IChatCompletionService chat,
    SchemaGenerator schemaGenerator,
    IConfiguration config)
{
    private readonly string _connectionString = config.GetConnectionString("DefaultConnection")!;

    public async Task<object> AskDatabaseAsync(string userQuestion)
    {
        // 1. Generate Schema dynamically
        string schemaDef = schemaGenerator.GetSchemaDefinition();

        // 2. Construct Prompt
        var systemPrompt = $@"
        You are a generic SQLite expert. 
        Your task is to convert the user's question into a SQL query based strictly on the schema provided.

        DATABASE SCHEMA:
        {schemaDef}

        RULES:
        1. Output ONLY the raw SQL query. Do not wrap in markdown (```sql).
        2. Use SQLite syntax.
        3. Only generate SELECT statements.
        4. If the question cannot be answered, return 'INVALID'.
        ";

        // 3. Call AI
        var history = new ChatHistory(systemPrompt);
        history.AddUserMessage(userQuestion);

        var response = await chat.GetChatMessageContentAsync(history);
        string sql = response.Content!.Replace("```sql", "").Replace("```", "").Trim();

        // 4. Validate & Execute
        if (sql.Equals("INVALID", StringComparison.OrdinalIgnoreCase))
            return new { Message = "I cannot answer that with the current data." };

        if (!sql.StartsWith("SELECT", StringComparison.OrdinalIgnoreCase))
            return new { Message = "Safety Error: Generated query was not a SELECT statement." };

        try
        {
            using var conn = new SqliteConnection(_connectionString);
            var results = await conn.QueryAsync(sql);
            return new { GeneratedSql = sql, Data = results };
        }
        catch (Exception ex)
        {
            return new { Error = ex.Message, SqlToCheck = sql };
        }
    }
}