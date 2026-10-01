namespace AtlasOps.Tools.Migration;

using Microsoft.Data.SqlClient;

using MongoDB.Driver;
using MySqlConnector;
using Npgsql;
using OpenSearch.Client;

public sealed record DatabaseProviderDescriptor(
    string Id,
    string DisplayName,
    Type ClientType,
    int DefaultPort,
    bool SupportsTransactions,
    bool SupportsDocumentQueries);

public static class DatabaseProviderCatalog
{
    public static IReadOnlyList<DatabaseProviderDescriptor> Providers { get; } =
    [
        new("sql-server", "Microsoft SQL Server", typeof(SqlConnection), 1433, true, false),
        new("postgresql", "PostgreSQL", typeof(NpgsqlConnection), 5432, true, false),
        new("mysql", "MySQL", typeof(MySqlConnection), 3306, true, false),
        new("mongodb", "MongoDB", typeof(MongoClient), 27017, true, true),
        new("opensearch", "OpenSearch", typeof(OpenSearchClient), 9200, false, true),
    ];
}
