namespace AtlasOps.Modules.Geography.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum BoundaryOverrideState { Draft, Active, Paused, Completed, Archived }
public sealed record BoundaryOverrideRecord(Guid Id, string Name, string Owner, BoundaryOverrideState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record BoundaryOverrideCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record BoundaryOverrideEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record BoundaryOverrideQuery(string? SearchText, BoundaryOverrideState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record BoundaryOverridePage(IReadOnlyList<BoundaryOverrideRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record BoundaryOverrideMutation(bool Succeeded, string Code, string Message, BoundaryOverrideRecord? Record, BoundaryOverrideEvent? Event);
public interface IBoundaryOverrideRepository
{
    ValueTask<BoundaryOverrideRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<BoundaryOverridePage> QueryAsync(BoundaryOverrideQuery query, CancellationToken cancellationToken);
    ValueTask<BoundaryOverrideMutation> SaveAsync(BoundaryOverrideRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface IBoundaryOverrideEventSink { ValueTask PublishAsync(BoundaryOverrideEvent domainEvent, CancellationToken cancellationToken); }