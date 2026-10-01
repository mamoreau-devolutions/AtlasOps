namespace AtlasOps.Modules.Geography.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum BoundaryComparisonState { Draft, Active, Paused, Completed, Archived }
public sealed record BoundaryComparisonRecord(Guid Id, string Name, string Owner, BoundaryComparisonState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record BoundaryComparisonCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record BoundaryComparisonEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record BoundaryComparisonQuery(string? SearchText, BoundaryComparisonState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record BoundaryComparisonPage(IReadOnlyList<BoundaryComparisonRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record BoundaryComparisonMutation(bool Succeeded, string Code, string Message, BoundaryComparisonRecord? Record, BoundaryComparisonEvent? Event);
public interface IBoundaryComparisonRepository
{
    ValueTask<BoundaryComparisonRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<BoundaryComparisonPage> QueryAsync(BoundaryComparisonQuery query, CancellationToken cancellationToken);
    ValueTask<BoundaryComparisonMutation> SaveAsync(BoundaryComparisonRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface IBoundaryComparisonEventSink { ValueTask PublishAsync(BoundaryComparisonEvent domainEvent, CancellationToken cancellationToken); }