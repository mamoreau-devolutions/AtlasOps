namespace AtlasOps.Modules.Lifecycle.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum VersionCatalogState { Draft, Active, Paused, Completed, Archived }
public sealed record VersionCatalogRecord(Guid Id, string Name, string Owner, VersionCatalogState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record VersionCatalogCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record VersionCatalogEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record VersionCatalogQuery(string? SearchText, VersionCatalogState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record VersionCatalogPage(IReadOnlyList<VersionCatalogRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record VersionCatalogMutation(bool Succeeded, string Code, string Message, VersionCatalogRecord? Record, VersionCatalogEvent? Event);
public interface IVersionCatalogRepository
{
    ValueTask<VersionCatalogRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<VersionCatalogPage> QueryAsync(VersionCatalogQuery query, CancellationToken cancellationToken);
    ValueTask<VersionCatalogMutation> SaveAsync(VersionCatalogRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface IVersionCatalogEventSink { ValueTask PublishAsync(VersionCatalogEvent domainEvent, CancellationToken cancellationToken); }