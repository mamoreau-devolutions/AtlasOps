namespace AtlasOps.Modules.Geography.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum BoundaryCatalogState { Draft, Active, Paused, Completed, Archived }
public sealed record BoundaryCatalogRecord(Guid Id, string Name, string Owner, BoundaryCatalogState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record BoundaryCatalogCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record BoundaryCatalogEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record BoundaryCatalogQuery(string? SearchText, BoundaryCatalogState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record BoundaryCatalogPage(IReadOnlyList<BoundaryCatalogRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record BoundaryCatalogMutation(bool Succeeded, string Code, string Message, BoundaryCatalogRecord? Record, BoundaryCatalogEvent? Event);
public interface IBoundaryCatalogRepository
{
    ValueTask<BoundaryCatalogRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<BoundaryCatalogPage> QueryAsync(BoundaryCatalogQuery query, CancellationToken cancellationToken);
    ValueTask<BoundaryCatalogMutation> SaveAsync(BoundaryCatalogRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface IBoundaryCatalogEventSink { ValueTask PublishAsync(BoundaryCatalogEvent domainEvent, CancellationToken cancellationToken); }