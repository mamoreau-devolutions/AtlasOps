namespace AtlasOps.Modules.NetworkIntelligence.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum AssignmentValidationState { Draft, Active, Paused, Completed, Archived }
public sealed record AssignmentValidationRecord(Guid Id, string Name, string Owner, AssignmentValidationState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record AssignmentValidationCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record AssignmentValidationEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record AssignmentValidationQuery(string? SearchText, AssignmentValidationState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record AssignmentValidationPage(IReadOnlyList<AssignmentValidationRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record AssignmentValidationMutation(bool Succeeded, string Code, string Message, AssignmentValidationRecord? Record, AssignmentValidationEvent? Event);
public interface IAssignmentValidationRepository
{
    ValueTask<AssignmentValidationRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<AssignmentValidationPage> QueryAsync(AssignmentValidationQuery query, CancellationToken cancellationToken);
    ValueTask<AssignmentValidationMutation> SaveAsync(AssignmentValidationRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface IAssignmentValidationEventSink { ValueTask PublishAsync(AssignmentValidationEvent domainEvent, CancellationToken cancellationToken); }