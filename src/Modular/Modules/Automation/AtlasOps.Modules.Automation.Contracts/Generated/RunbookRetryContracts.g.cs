namespace AtlasOps.Modules.Automation.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum RunbookRetryState { Draft, Active, Paused, Completed, Archived }
public sealed record RunbookRetryRecord(Guid Id, string Name, string Owner, RunbookRetryState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record RunbookRetryCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record RunbookRetryEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record RunbookRetryQuery(string? SearchText, RunbookRetryState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record RunbookRetryPage(IReadOnlyList<RunbookRetryRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record RunbookRetryMutation(bool Succeeded, string Code, string Message, RunbookRetryRecord? Record, RunbookRetryEvent? Event);
public interface IRunbookRetryRepository
{
    ValueTask<RunbookRetryRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<RunbookRetryPage> QueryAsync(RunbookRetryQuery query, CancellationToken cancellationToken);
    ValueTask<RunbookRetryMutation> SaveAsync(RunbookRetryRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface IRunbookRetryEventSink { ValueTask PublishAsync(RunbookRetryEvent domainEvent, CancellationToken cancellationToken); }