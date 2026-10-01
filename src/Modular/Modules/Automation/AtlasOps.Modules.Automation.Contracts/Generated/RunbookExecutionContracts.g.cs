namespace AtlasOps.Modules.Automation.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum RunbookExecutionState { Draft, Active, Paused, Completed, Archived }
public sealed record RunbookExecutionRecord(Guid Id, string Name, string Owner, RunbookExecutionState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record RunbookExecutionCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record RunbookExecutionEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record RunbookExecutionQuery(string? SearchText, RunbookExecutionState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record RunbookExecutionPage(IReadOnlyList<RunbookExecutionRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record RunbookExecutionMutation(bool Succeeded, string Code, string Message, RunbookExecutionRecord? Record, RunbookExecutionEvent? Event);
public interface IRunbookExecutionRepository
{
    ValueTask<RunbookExecutionRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<RunbookExecutionPage> QueryAsync(RunbookExecutionQuery query, CancellationToken cancellationToken);
    ValueTask<RunbookExecutionMutation> SaveAsync(RunbookExecutionRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface IRunbookExecutionEventSink { ValueTask PublishAsync(RunbookExecutionEvent domainEvent, CancellationToken cancellationToken); }