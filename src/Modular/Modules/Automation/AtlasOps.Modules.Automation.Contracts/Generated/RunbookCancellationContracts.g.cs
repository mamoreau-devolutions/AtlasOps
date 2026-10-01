namespace AtlasOps.Modules.Automation.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum RunbookCancellationState { Draft, Active, Paused, Completed, Archived }
public sealed record RunbookCancellationRecord(Guid Id, string Name, string Owner, RunbookCancellationState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record RunbookCancellationCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record RunbookCancellationEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record RunbookCancellationQuery(string? SearchText, RunbookCancellationState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record RunbookCancellationPage(IReadOnlyList<RunbookCancellationRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record RunbookCancellationMutation(bool Succeeded, string Code, string Message, RunbookCancellationRecord? Record, RunbookCancellationEvent? Event);
public interface IRunbookCancellationRepository
{
    ValueTask<RunbookCancellationRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<RunbookCancellationPage> QueryAsync(RunbookCancellationQuery query, CancellationToken cancellationToken);
    ValueTask<RunbookCancellationMutation> SaveAsync(RunbookCancellationRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface IRunbookCancellationEventSink { ValueTask PublishAsync(RunbookCancellationEvent domainEvent, CancellationToken cancellationToken); }