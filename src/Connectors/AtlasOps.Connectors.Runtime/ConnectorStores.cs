namespace AtlasOps.Connectors.Runtime;

using System.Collections.Concurrent;

using AtlasOps.Connectors.Contracts;

public sealed class InMemoryConnectorCheckpointStore : IConnectorCheckpointStore
{
    private readonly ConcurrentDictionary<string, ConnectorCheckpoint> checkpoints =
        new(StringComparer.OrdinalIgnoreCase);

    public ValueTask<ConnectorCheckpoint?> GetAsync(
        string connectorId,
        string partition,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        this.checkpoints.TryGetValue(CreateKey(connectorId, partition), out ConnectorCheckpoint? checkpoint);
        return ValueTask.FromResult(checkpoint);
    }

    public ValueTask<bool> SaveAsync(
        ConnectorCheckpoint checkpoint,
        long expectedRevision,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        string key = CreateKey(checkpoint.ConnectorId, checkpoint.Partition);

        while (true)
        {
            if (!this.checkpoints.TryGetValue(key, out ConnectorCheckpoint? existing))
            {
                if (expectedRevision != 0 || checkpoint.Revision != 1)
                {
                    return ValueTask.FromResult(false);
                }

                if (this.checkpoints.TryAdd(key, checkpoint))
                {
                    return ValueTask.FromResult(true);
                }

                continue;
            }

            if (existing.Revision != expectedRevision || checkpoint.Revision != existing.Revision + 1)
            {
                return ValueTask.FromResult(false);
            }

            if (this.checkpoints.TryUpdate(key, checkpoint, existing))
            {
                return ValueTask.FromResult(true);
            }
        }
    }

    private static string CreateKey(string connectorId, string partition)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(connectorId);
        ArgumentException.ThrowIfNullOrWhiteSpace(partition);
        return string.Concat(connectorId, "\u001f", partition);
    }
}

public sealed class InMemoryConnectorAuditSink(int capacity = 10_000) : IConnectorAuditSink
{
    private readonly object sync = new();
    private readonly Queue<ConnectorAuditRecord> records = new();

    public IReadOnlyList<ConnectorAuditRecord> Records
    {
        get
        {
            lock (this.sync)
            {
                return this.records.ToArray();
            }
        }
    }

    public ValueTask PublishAsync(ConnectorAuditRecord record, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        lock (this.sync)
        {
            this.records.Enqueue(record);
            while (this.records.Count > capacity)
            {
                this.records.Dequeue();
            }
        }

        return ValueTask.CompletedTask;
    }
}
