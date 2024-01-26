using AdventureWorks2022.Models;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Standard.AI.OpenAI.Clients.OpenAIs;
using Standard.AI.OpenAI.Models.Configurations;
using Standard.AI.OpenAI.Models.Services.Foundations.Completions;
using System.Globalization;

namespace AdventureWorks2022
{
    public interface IADataClient
    {
        public Task<IReadOnlyList<Person>> RunAiQueryAsync(string query, string tableName);
    }

    public class AiDataClient(AdventureWorks2012Context db, ILogger<AiDataClient> logger, IConfiguration configurations) : IADataClient
    {
        private readonly AdventureWorks2012Context _db = db;
        private readonly ILogger<AiDataClient> _logger = logger;
        private readonly string _openAiKey = configurations.GetConnectionString("OpenAiKey")!;
        private readonly string _apiUrl = configurations.GetConnectionString("OpenAiUrl")!;
        private readonly string _sqlConnectionString = configurations.GetConnectionString("SqlServerAppDBSettings")!;

        public async Task<IReadOnlyList<Person>> RunAiQueryAsync(string query, string tableName)
        {
            try
            {
                var openAiConfigurations = new OpenAIConfigurations
                {
                    ApiKey = _openAiKey,
                    ApiUrl = _apiUrl
                };

                IOpenAIClient openAiClient =
                    new OpenAIClient(openAiConfigurations);

                // Set the culture to French in France
                if (CultureInfo.CurrentCulture == CultureInfo.InvariantCulture)
                {
                    Console.WriteLine("Globalization Invariant Mode is enabled");
                }
                else
                {
                    Console.WriteLine("Globalization Invariant Mode is not enabled");
                }
                
                var allTablesSql = _db.Database.SqlQuery<SchemaTable>(@$"select	    NEWID() as Id,
                                                                                    t.[name] as TableName, 
		                                                                            c.[name] as ColumnName,
		                                                                            ty.[name] as DataType,
		                                                                            sc.[name] as SchemaName
                                                                            FROM sys.tables t
                                                                            inner join sys.columns c on c.object_id = t.object_id
                                                                            inner join sys.types ty on ty.system_type_id = c.system_type_id
                                                                            inner join sys.schemas sc on sc.schema_id = t.schema_id
                                                                            where t.[name] LIKE 'Person'");

                    var byTables = allTablesSql.ToList().GroupBy(t => t.TableName);
                    var fullQuery = GetDescriptiveOfAllTables(byTables);

                    var inputCompletion = new Completion
                    {
                        Request = new CompletionRequest
                        {
                            Prompts =
                            ["@{\"role\": \"system\","+
                                "\"content\": \"Given the following SQL tables, your job is to write queries given a user’s request."+
                                $" {fullQuery} " +
                                    "}," +     
                            "{" +
                                "\"role\": \"user\","+
                                "\"content\":" + $"\"Translate the following request into a SQL query: {query}\"" +
                            "\"}"],

                            Model = "gpt-3.5-turbo-instruct",
                            MaxTokens = 100
                        }
                    };

                    var result = await openAiClient.Completions.PromptCompletionAsync(inputCompletion);
                    var sql = result.Response.Choices[0].Text;

                    if (sql.Contains("SELECT") is true)
                    {
                        if (sql.IndexOf("SELECT", StringComparison.Ordinal) is not 0)
                            sql = sql.Remove(0, sql.IndexOf("SELECT", StringComparison.Ordinal));

                        var students = _db.Person.FromSqlRaw(sql).ToList();

                        return students;
                    }

                    throw new Exception("Couldn't process request. Please try again.");
                
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while running AI query");
                throw new Exception("Couldn't process request. Please try again.");
            }
        }

        private static string GetDescriptiveOfAllTables(IEnumerable<IGrouping<dynamic, dynamic>> allTables)
        {
            var properties = string.Empty;

            foreach (var table in allTables)
            {
                properties += $"Entity name: {table.Key} has the following properties: ";

                properties = table.Aggregate(properties,
                    (current, property) => current + $"{property.ColumnName} as {property.DataType}.");
            }

            return properties;
        }
    }

    //public record SchemaTable(string TableName, string ColumnName, string DataType);
}
