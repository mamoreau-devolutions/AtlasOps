namespace AtlasOps.Modules.Lifecycle.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum ReleaseCatalogState { Draft, Active, Paused, Completed, Archived }
public sealed record ReleaseCatalogRecord(Guid Id, string Name, string Owner, ReleaseCatalogState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record ReleaseCatalogCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record ReleaseCatalogEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record ReleaseCatalogQuery(string? SearchText, ReleaseCatalogState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record ReleaseCatalogPage(IReadOnlyList<ReleaseCatalogRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record ReleaseCatalogMutation(bool Succeeded, string Code, string Message, ReleaseCatalogRecord? Record, ReleaseCatalogEvent? Event);
public interface IReleaseCatalogRepository
{
    ValueTask<ReleaseCatalogRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<ReleaseCatalogPage> QueryAsync(ReleaseCatalogQuery query, CancellationToken cancellationToken);
    ValueTask<ReleaseCatalogMutation> SaveAsync(ReleaseCatalogRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface IReleaseCatalogEventSink { ValueTask PublishAsync(ReleaseCatalogEvent domainEvent, CancellationToken cancellationToken); }