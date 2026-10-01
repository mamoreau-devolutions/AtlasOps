namespace AtlasOps.Connectors.Data;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

using Newtonsoft.Json.Bson;

using OpenMcdf;

using Snappier;

public sealed record DataProviderDescriptor(string Id, Type ProviderType, string Capability);

public sealed record RetentionDecision(Guid Id, string EntityType, DateTimeOffset ExpiresAt, string Reason);

public static class ConnectorDataProviderCatalog
{
    public static IReadOnlyList<DataProviderDescriptor> Providers { get; } =
    [
        new("ef-core", typeof(DbContext), "Relational object mapping"),
        new("sqlite", typeof(SqliteDbContextOptionsBuilderExtensions), "Embedded connector persistence"),
        new("sql-server", typeof(SqlServerDbContextOptionsExtensions), "SQL Server connector persistence"),
        new("postgresql", typeof(NpgsqlDbContextOptionsBuilderExtensions), "PostgreSQL connector persistence"),
        new("bson", typeof(BsonDataReader), "Binary JSON interchange"),
        new("compound-file", typeof(Storage), "Compound document storage"),
        new("snappy", typeof(Snappy), "Fast payload compression"),
    ];
}

public sealed class ConnectorSnapshotCache : IDisposable
{
    private readonly MemoryCache cache = new(new MemoryCacheOptions
    {
        SizeLimit = 10_000,
    });

    public void Set<T>(string connectorId, string key, T value, TimeSpan lifetime)
        where T : notnull
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(connectorId);
        ArgumentException.ThrowIfNullOrWhiteSpace(key);
        if (lifetime <= TimeSpan.Zero)
        {
            throw new ArgumentOutOfRangeException(nameof(lifetime));
        }

        this.cache.Set(CreateKey(connectorId, key), value, new MemoryCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = lifetime,
            Size = 1,
        });
    }

    public bool TryGet<T>(string connectorId, string key, out T? value)
    {
        return this.cache.TryGetValue(CreateKey(connectorId, key), out value);
    }

    public void Remove(string connectorId, string key)
    {
        this.cache.Remove(CreateKey(connectorId, key));
    }

    public void Dispose()
    {
        this.cache.Dispose();
    }

    private static string CreateKey(string connectorId, string key)
    {
        return string.Concat(connectorId, "\u001f", key);
    }
}

public sealed class ConnectorRetentionPlanner
{
    public IReadOnlyList<RetentionDecision> Plan(
        IEnumerable<ConnectorOutboxEntity> records,
        DateTimeOffset now,
        TimeSpan processedRetention,
        TimeSpan failedRetention)
    {
        ArgumentNullException.ThrowIfNull(records);
        if (processedRetention < TimeSpan.Zero || failedRetention < TimeSpan.Zero)
        {
            throw new ArgumentOutOfRangeException(nameof(processedRetention));
        }

        return records.Select(
                item =>
                {
                    TimeSpan retention = item.ProcessedAt.HasValue ? processedRetention : failedRetention;
                    DateTimeOffset basis = item.ProcessedAt ?? item.OccurredAt;
                    return new RetentionDecision(
                        item.Id,
                        nameof(ConnectorOutboxEntity),
                        basis + retention,
                        item.ProcessedAt.HasValue ? "processed" : "unprocessed");
                })
            .Where(item => item.ExpiresAt <= now)
            .OrderBy(static item => item.ExpiresAt)
            .ToArray();
    }
}
