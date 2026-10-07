namespace AtlasOps.Connectors.Data;

using System.Globalization;

using Microsoft.Data.Sqlite;

/// <summary>
/// Connector persistence over parameterized ADO.NET commands. The schema is declared explicitly
/// instead of being inferred through an ORM model, which keeps the store trimming and NativeAOT safe.
/// </summary>
public sealed class ConnectorDataStore : IAsyncDisposable, IDisposable
{
    private const string SchemaProbeSql =
        "SELECT COUNT(*) FROM sqlite_master WHERE type = 'table' AND name IN ('connector_states', 'connector_checkpoints', 'connector_outbox')";

    private const string CreateSchemaSql = """
        CREATE TABLE IF NOT EXISTS connector_states (
            id TEXT NOT NULL PRIMARY KEY,
            connector_id TEXT NOT NULL CHECK (length(connector_id) <= 120),
            display_name TEXT NOT NULL CHECK (length(display_name) <= 160),
            configuration_json TEXT NOT NULL,
            enabled INTEGER NOT NULL,
            revision INTEGER NOT NULL,
            updated_at TEXT NOT NULL
        );
        CREATE UNIQUE INDEX IF NOT EXISTS ix_connector_states_connector_id
            ON connector_states(connector_id);
        CREATE TABLE IF NOT EXISTS connector_checkpoints (
            id TEXT NOT NULL PRIMARY KEY,
            connector_id TEXT NOT NULL CHECK (length(connector_id) <= 120),
            partition TEXT NOT NULL CHECK (length(partition) <= 200),
            cursor TEXT NOT NULL CHECK (length(cursor) <= 2000),
            revision INTEGER NOT NULL,
            updated_at TEXT NOT NULL
        );
        CREATE UNIQUE INDEX IF NOT EXISTS ix_connector_checkpoints_connector_partition
            ON connector_checkpoints(connector_id, partition);
        CREATE TABLE IF NOT EXISTS connector_outbox (
            id TEXT NOT NULL PRIMARY KEY,
            event_type TEXT NOT NULL CHECK (length(event_type) <= 200),
            payload_json TEXT NOT NULL,
            occurred_at TEXT NOT NULL,
            processed_at TEXT NULL,
            attempt_count INTEGER NOT NULL
        );
        CREATE INDEX IF NOT EXISTS ix_connector_outbox_processed_occurred
            ON connector_outbox(processed_at, occurred_at);
        """;

    private readonly SqliteConnection connection;
    private readonly bool ownsConnection;

    public ConnectorDataStore(string connectionString)
        : this(new SqliteConnection(connectionString), ownsConnection: true)
    {
    }

    public ConnectorDataStore(SqliteConnection connection, bool ownsConnection = false)
    {
        ArgumentNullException.ThrowIfNull(connection);
        this.connection = connection;
        this.ownsConnection = ownsConnection;
    }

    public async Task<bool> EnsureCreatedAsync(CancellationToken cancellationToken = default)
    {
        await this.OpenAsync(cancellationToken).ConfigureAwait(false);
        await using SqliteCommand probe = this.CreateCommand(SchemaProbeSql);
        long existingTables = Convert.ToInt64(
            await probe.ExecuteScalarAsync(cancellationToken).ConfigureAwait(false),
            CultureInfo.InvariantCulture);
        if (existingTables == 3)
        {
            return false;
        }

        await using SqliteCommand create = this.CreateCommand(CreateSchemaSql);
        await create.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);
        return true;
    }

    public async Task AddStateAsync(ConnectorStateEntity state, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(state);
        await this.OpenAsync(cancellationToken).ConfigureAwait(false);
        await using SqliteCommand command = this.CreateCommand("""
            INSERT INTO connector_states(id, connector_id, display_name, configuration_json, enabled, revision, updated_at)
            VALUES ($id, $connectorId, $displayName, $configurationJson, $enabled, $revision, $updatedAt);
            """);
        command.Parameters.AddWithValue("$id", state.Id.ToString("D"));
        command.Parameters.AddWithValue("$connectorId", state.ConnectorId);
        command.Parameters.AddWithValue("$displayName", state.DisplayName);
        command.Parameters.AddWithValue("$configurationJson", state.ConfigurationJson);
        command.Parameters.AddWithValue("$enabled", state.Enabled ? 1 : 0);
        command.Parameters.AddWithValue("$revision", state.Revision);
        command.Parameters.AddWithValue("$updatedAt", FormatTimestamp(state.UpdatedAt));
        await command.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);
    }

    public async Task<int> CountStatesAsync(CancellationToken cancellationToken = default)
    {
        await this.OpenAsync(cancellationToken).ConfigureAwait(false);
        await using SqliteCommand command = this.CreateCommand("SELECT COUNT(*) FROM connector_states;");
        return Convert.ToInt32(
            await command.ExecuteScalarAsync(cancellationToken).ConfigureAwait(false),
            CultureInfo.InvariantCulture);
    }

    public async Task<IReadOnlyList<ConnectorStateEntity>> ReadStatesAsync(CancellationToken cancellationToken = default)
    {
        await this.OpenAsync(cancellationToken).ConfigureAwait(false);
        await using SqliteCommand command = this.CreateCommand("""
            SELECT id, connector_id, display_name, configuration_json, enabled, revision, updated_at
            FROM connector_states
            ORDER BY connector_id;
            """);
        await using SqliteDataReader reader = await command.ExecuteReaderAsync(cancellationToken).ConfigureAwait(false);
        List<ConnectorStateEntity> states = [];
        while (await reader.ReadAsync(cancellationToken).ConfigureAwait(false))
        {
            states.Add(new ConnectorStateEntity
            {
                Id = Guid.Parse(reader.GetString(0)),
                ConnectorId = reader.GetString(1),
                DisplayName = reader.GetString(2),
                ConfigurationJson = reader.GetString(3),
                Enabled = reader.GetInt64(4) != 0,
                Revision = reader.GetInt64(5),
                UpdatedAt = ParseTimestamp(reader.GetString(6)),
            });
        }

        return states;
    }

    /// <summary>
    /// Stores a checkpoint cursor using optimistic concurrency: the write succeeds only when
    /// <paramref name="expectedRevision"/> matches the stored revision (zero for a new partition).
    /// </summary>
    public async Task<bool> TrySaveCheckpointAsync(
        string connectorId,
        string partition,
        string cursor,
        long expectedRevision,
        DateTimeOffset updatedAt,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(connectorId);
        ArgumentException.ThrowIfNullOrWhiteSpace(partition);
        ArgumentNullException.ThrowIfNull(cursor);
        ArgumentOutOfRangeException.ThrowIfNegative(expectedRevision);
        await this.OpenAsync(cancellationToken).ConfigureAwait(false);
        await using SqliteCommand command = expectedRevision == 0
            ? this.CreateCommand("""
                INSERT INTO connector_checkpoints(id, connector_id, partition, cursor, revision, updated_at)
                VALUES ($id, $connectorId, $partition, $cursor, 1, $updatedAt)
                ON CONFLICT(connector_id, partition) DO NOTHING;
                """)
            : this.CreateCommand("""
                UPDATE connector_checkpoints
                SET cursor = $cursor, revision = revision + 1, updated_at = $updatedAt
                WHERE connector_id = $connectorId AND partition = $partition AND revision = $expectedRevision;
                """);
        command.Parameters.AddWithValue("$id", Guid.NewGuid().ToString("D"));
        command.Parameters.AddWithValue("$connectorId", connectorId);
        command.Parameters.AddWithValue("$partition", partition);
        command.Parameters.AddWithValue("$cursor", cursor);
        command.Parameters.AddWithValue("$expectedRevision", expectedRevision);
        command.Parameters.AddWithValue("$updatedAt", FormatTimestamp(updatedAt));
        return await command.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false) == 1;
    }

    public async Task<ConnectorCheckpointEntity?> ReadCheckpointAsync(
        string connectorId,
        string partition,
        CancellationToken cancellationToken = default)
    {
        await this.OpenAsync(cancellationToken).ConfigureAwait(false);
        await using SqliteCommand command = this.CreateCommand("""
            SELECT id, connector_id, partition, cursor, revision, updated_at
            FROM connector_checkpoints
            WHERE connector_id = $connectorId AND partition = $partition;
            """);
        command.Parameters.AddWithValue("$connectorId", connectorId);
        command.Parameters.AddWithValue("$partition", partition);
        await using SqliteDataReader reader = await command.ExecuteReaderAsync(cancellationToken).ConfigureAwait(false);
        if (!await reader.ReadAsync(cancellationToken).ConfigureAwait(false))
        {
            return null;
        }

        return new ConnectorCheckpointEntity
        {
            Id = Guid.Parse(reader.GetString(0)),
            ConnectorId = reader.GetString(1),
            Partition = reader.GetString(2),
            Cursor = reader.GetString(3),
            Revision = reader.GetInt64(4),
            UpdatedAt = ParseTimestamp(reader.GetString(5)),
        };
    }

    public async Task AddOutboxAsync(ConnectorOutboxEntity entry, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(entry);
        await this.OpenAsync(cancellationToken).ConfigureAwait(false);
        await using SqliteCommand command = this.CreateCommand("""
            INSERT INTO connector_outbox(id, event_type, payload_json, occurred_at, processed_at, attempt_count)
            VALUES ($id, $eventType, $payloadJson, $occurredAt, $processedAt, $attemptCount);
            """);
        command.Parameters.AddWithValue("$id", entry.Id.ToString("D"));
        command.Parameters.AddWithValue("$eventType", entry.EventType);
        command.Parameters.AddWithValue("$payloadJson", entry.PayloadJson);
        command.Parameters.AddWithValue("$occurredAt", FormatTimestamp(entry.OccurredAt));
        command.Parameters.AddWithValue(
            "$processedAt",
            entry.ProcessedAt is { } processedAt ? FormatTimestamp(processedAt) : DBNull.Value);
        command.Parameters.AddWithValue("$attemptCount", entry.AttemptCount);
        await command.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);
    }

    public async Task<IReadOnlyList<ConnectorOutboxEntity>> ReadPendingOutboxAsync(
        int limit,
        CancellationToken cancellationToken = default)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(limit, 1);
        await this.OpenAsync(cancellationToken).ConfigureAwait(false);
        await using SqliteCommand command = this.CreateCommand("""
            SELECT id, event_type, payload_json, occurred_at, processed_at, attempt_count
            FROM connector_outbox
            WHERE processed_at IS NULL
            ORDER BY occurred_at
            LIMIT $limit;
            """);
        command.Parameters.AddWithValue("$limit", limit);
        await using SqliteDataReader reader = await command.ExecuteReaderAsync(cancellationToken).ConfigureAwait(false);
        List<ConnectorOutboxEntity> entries = [];
        while (await reader.ReadAsync(cancellationToken).ConfigureAwait(false))
        {
            entries.Add(new ConnectorOutboxEntity
            {
                Id = Guid.Parse(reader.GetString(0)),
                EventType = reader.GetString(1),
                PayloadJson = reader.GetString(2),
                OccurredAt = ParseTimestamp(reader.GetString(3)),
                ProcessedAt = reader.IsDBNull(4) ? null : ParseTimestamp(reader.GetString(4)),
                AttemptCount = reader.GetInt32(5),
            });
        }

        return entries;
    }

    public async Task<bool> MarkOutboxProcessedAsync(
        Guid id,
        DateTimeOffset processedAt,
        CancellationToken cancellationToken = default)
    {
        await this.OpenAsync(cancellationToken).ConfigureAwait(false);
        await using SqliteCommand command = this.CreateCommand("""
            UPDATE connector_outbox
            SET processed_at = $processedAt, attempt_count = attempt_count + 1
            WHERE id = $id AND processed_at IS NULL;
            """);
        command.Parameters.AddWithValue("$id", id.ToString("D"));
        command.Parameters.AddWithValue("$processedAt", FormatTimestamp(processedAt));
        return await command.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false) == 1;
    }

    public void Dispose()
    {
        if (this.ownsConnection)
        {
            this.connection.Dispose();
        }
    }

    public async ValueTask DisposeAsync()
    {
        if (this.ownsConnection)
        {
            await this.connection.DisposeAsync().ConfigureAwait(false);
        }
    }

    private static string FormatTimestamp(DateTimeOffset value)
    {
        return value.ToUniversalTime().ToString("O", CultureInfo.InvariantCulture);
    }

    private static DateTimeOffset ParseTimestamp(string value)
    {
        return DateTimeOffset.Parse(value, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind);
    }

    private async Task OpenAsync(CancellationToken cancellationToken)
    {
        if (this.connection.State != System.Data.ConnectionState.Open)
        {
            await this.connection.OpenAsync(cancellationToken).ConfigureAwait(false);
        }
    }

    private SqliteCommand CreateCommand(string sql)
    {
        SqliteCommand command = this.connection.CreateCommand();
        command.CommandText = sql;
        return command;
    }
}

public sealed class ConnectorStateEntity
{
    public Guid Id { get; set; }

    public required string ConnectorId { get; set; }

    public required string DisplayName { get; set; }

    public required string ConfigurationJson { get; set; }

    public bool Enabled { get; set; }

    public long Revision { get; set; }

    public DateTimeOffset UpdatedAt { get; set; }
}

public sealed class ConnectorCheckpointEntity
{
    public Guid Id { get; set; }

    public required string ConnectorId { get; set; }

    public required string Partition { get; set; }

    public required string Cursor { get; set; }

    public long Revision { get; set; }

    public DateTimeOffset UpdatedAt { get; set; }
}

public sealed class ConnectorOutboxEntity
{
    public Guid Id { get; set; }

    public required string EventType { get; set; }

    public required string PayloadJson { get; set; }

    public DateTimeOffset OccurredAt { get; set; }

    public DateTimeOffset? ProcessedAt { get; set; }

    public int AttemptCount { get; set; }
}
