namespace AtlasOps.Modules.Deployments.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum RollbackValidationState { Draft, Active, Paused, Completed, Archived }
public sealed record RollbackValidationRecord(Guid Id, string Name, string Owner, RollbackValidationState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record RollbackValidationCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record RollbackValidationEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record RollbackValidationQuery(string? SearchText, RollbackValidationState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record RollbackValidationPage(IReadOnlyList<RollbackValidationRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record RollbackValidationMutation(bool Succeeded, string Code, string Message, RollbackValidationRecord? Record, RollbackValidationEvent? Event);
public interface IRollbackValidationRepository
{
    ValueTask<RollbackValidationRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<RollbackValidationPage> QueryAsync(RollbackValidationQuery query, CancellationToken cancellationToken);
    ValueTask<RollbackValidationMutation> SaveAsync(RollbackValidationRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface IRollbackValidationEventSink { ValueTask PublishAsync(RollbackValidationEvent domainEvent, CancellationToken cancellationToken); }