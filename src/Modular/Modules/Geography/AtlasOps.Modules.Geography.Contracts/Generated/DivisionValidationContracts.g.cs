namespace AtlasOps.Modules.Geography.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum DivisionValidationState { Draft, Active, Paused, Completed, Archived }
public sealed record DivisionValidationRecord(Guid Id, string Name, string Owner, DivisionValidationState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record DivisionValidationCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record DivisionValidationEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record DivisionValidationQuery(string? SearchText, DivisionValidationState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record DivisionValidationPage(IReadOnlyList<DivisionValidationRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record DivisionValidationMutation(bool Succeeded, string Code, string Message, DivisionValidationRecord? Record, DivisionValidationEvent? Event);
public interface IDivisionValidationRepository
{
    ValueTask<DivisionValidationRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<DivisionValidationPage> QueryAsync(DivisionValidationQuery query, CancellationToken cancellationToken);
    ValueTask<DivisionValidationMutation> SaveAsync(DivisionValidationRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface IDivisionValidationEventSink { ValueTask PublishAsync(DivisionValidationEvent domainEvent, CancellationToken cancellationToken); }