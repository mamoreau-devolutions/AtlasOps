namespace AtlasOps.Adapters.Persistence.Sqlite;

using System.Globalization;

using Microsoft.Data.Sqlite;

public sealed record SqliteOperationEntry(
    long Sequence,
    string Category,
    string Operation,
    string ResourceId,
    DateTimeOffset OccurredAt,
    string Payload);

public sealed class SqliteOperationJournal
{
    private readonly string connectionString;

    public SqliteOperationJournal(string databasePath)
    {
        SqliteConnectionStringBuilder builder = new()
        {
            DataSource = databasePath,
            Mode = SqliteOpenMode.ReadWriteCreate,
            Cache = SqliteCacheMode.Shared,
            ForeignKeys = true,
            Pooling = false,
        };
        this.connectionString = builder.ToString();
    }

    public async Task InitializeAsync(CancellationToken cancellationToken)
    {
        await using SqliteConnection connection = new(this.connectionString);
        await connection.OpenAsync(cancellationToken).ConfigureAwait(false);
        await using SqliteCommand command = connection.CreateCommand();
        command.CommandText = """
            CREATE TABLE IF NOT EXISTS operation_journal (
                sequence INTEGER PRIMARY KEY AUTOINCREMENT,
                category TEXT NOT NULL,
                operation TEXT NOT NULL,
                resource_id TEXT NOT NULL,
                occurred_at TEXT NOT NULL,
                payload TEXT NOT NULL
            );
            CREATE INDEX IF NOT EXISTS ix_operation_journal_resource
                ON operation_journal(resource_id, sequence);
            """;
        await command.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);
    }

    public async Task<long> AppendAsync(
        string category,
        string operation,
        string resourceId,
        DateTimeOffset occurredAt,
        string payload,
        CancellationToken cancellationToken)
    {
        await using SqliteConnection connection = new(this.connectionString);
        await connection.OpenAsync(cancellationToken).ConfigureAwait(false);
        await using SqliteTransaction transaction = connection.BeginTransaction();
        await using SqliteCommand command = connection.CreateCommand();
        command.Transaction = transaction;
        command.CommandText = """
            INSERT INTO operation_journal(category, operation, resource_id, occurred_at, payload)
            VALUES ($category, $operation, $resourceId, $occurredAt, $payload);
            SELECT last_insert_rowid();
            """;
        command.Parameters.AddWithValue("$category", category);
        command.Parameters.AddWithValue("$operation", operation);
        command.Parameters.AddWithValue("$resourceId", resourceId);
        command.Parameters.AddWithValue("$occurredAt", occurredAt.ToUniversalTime().ToString("O"));
        command.Parameters.AddWithValue("$payload", payload);
        object? result = await command.ExecuteScalarAsync(cancellationToken).ConfigureAwait(false);
        await transaction.CommitAsync(cancellationToken).ConfigureAwait(false);
        return Convert.ToInt64(result);
    }

    public async Task<IReadOnlyList<SqliteOperationEntry>> ReadResourceAsync(
        string resourceId,
        int limit,
        CancellationToken cancellationToken)
    {
        List<SqliteOperationEntry> entries = [];
        await using SqliteConnection connection = new(this.connectionString);
        await connection.OpenAsync(cancellationToken).ConfigureAwait(false);
        await using SqliteCommand command = connection.CreateCommand();
        command.CommandText = """
            SELECT sequence, category, operation, resource_id, occurred_at, payload
            FROM operation_journal
            WHERE resource_id = $resourceId
            ORDER BY sequence DESC
            LIMIT $limit;
            """;
        command.Parameters.AddWithValue("$resourceId", resourceId);
        command.Parameters.AddWithValue("$limit", Math.Clamp(limit, 1, 1_000));
        await using SqliteDataReader reader = await command.ExecuteReaderAsync(cancellationToken).ConfigureAwait(false);
        while (await reader.ReadAsync(cancellationToken).ConfigureAwait(false))
        {
            entries.Add(new(
                reader.GetInt64(0),
                reader.GetString(1),
                reader.GetString(2),
                reader.GetString(3),
                DateTimeOffset.Parse(reader.GetString(4), CultureInfo.InvariantCulture),
                reader.GetString(5)));
        }

        return entries;
    }
}
