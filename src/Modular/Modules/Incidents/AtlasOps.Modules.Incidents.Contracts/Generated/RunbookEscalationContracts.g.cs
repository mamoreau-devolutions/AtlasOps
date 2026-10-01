namespace AtlasOps.Modules.Incidents.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum RunbookEscalationState { Draft, Active, Paused, Completed, Archived }
public sealed record RunbookEscalationRecord(Guid Id, string Name, string Owner, RunbookEscalationState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record RunbookEscalationCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record RunbookEscalationEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record RunbookEscalationQuery(string? SearchText, RunbookEscalationState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record RunbookEscalationPage(IReadOnlyList<RunbookEscalationRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record RunbookEscalationMutation(bool Succeeded, string Code, string Message, RunbookEscalationRecord? Record, RunbookEscalationEvent? Event);
public interface IRunbookEscalationRepository
{
    ValueTask<RunbookEscalationRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<RunbookEscalationPage> QueryAsync(RunbookEscalationQuery query, CancellationToken cancellationToken);
    ValueTask<RunbookEscalationMutation> SaveAsync(RunbookEscalationRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface IRunbookEscalationEventSink { ValueTask PublishAsync(RunbookEscalationEvent domainEvent, CancellationToken cancellationToken); }