namespace AtlasOps.Modules.Geography.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum BoundaryValidationState { Draft, Active, Paused, Completed, Archived }
public sealed record BoundaryValidationRecord(Guid Id, string Name, string Owner, BoundaryValidationState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record BoundaryValidationCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record BoundaryValidationEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record BoundaryValidationQuery(string? SearchText, BoundaryValidationState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record BoundaryValidationPage(IReadOnlyList<BoundaryValidationRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record BoundaryValidationMutation(bool Succeeded, string Code, string Message, BoundaryValidationRecord? Record, BoundaryValidationEvent? Event);
public interface IBoundaryValidationRepository
{
    ValueTask<BoundaryValidationRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<BoundaryValidationPage> QueryAsync(BoundaryValidationQuery query, CancellationToken cancellationToken);
    ValueTask<BoundaryValidationMutation> SaveAsync(BoundaryValidationRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface IBoundaryValidationEventSink { ValueTask PublishAsync(BoundaryValidationEvent domainEvent, CancellationToken cancellationToken); }