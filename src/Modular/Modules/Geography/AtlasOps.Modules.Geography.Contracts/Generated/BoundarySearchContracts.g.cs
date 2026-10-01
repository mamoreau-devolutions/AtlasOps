namespace AtlasOps.Modules.Geography.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum BoundarySearchState { Draft, Active, Paused, Completed, Archived }
public sealed record BoundarySearchRecord(Guid Id, string Name, string Owner, BoundarySearchState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record BoundarySearchCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record BoundarySearchEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record BoundarySearchQuery(string? SearchText, BoundarySearchState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record BoundarySearchPage(IReadOnlyList<BoundarySearchRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record BoundarySearchMutation(bool Succeeded, string Code, string Message, BoundarySearchRecord? Record, BoundarySearchEvent? Event);
public interface IBoundarySearchRepository
{
    ValueTask<BoundarySearchRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<BoundarySearchPage> QueryAsync(BoundarySearchQuery query, CancellationToken cancellationToken);
    ValueTask<BoundarySearchMutation> SaveAsync(BoundarySearchRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface IBoundarySearchEventSink { ValueTask PublishAsync(BoundarySearchEvent domainEvent, CancellationToken cancellationToken); }