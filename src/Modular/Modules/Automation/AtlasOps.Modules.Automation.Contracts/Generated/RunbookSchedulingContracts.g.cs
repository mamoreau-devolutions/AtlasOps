namespace AtlasOps.Modules.Automation.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum RunbookSchedulingState { Draft, Active, Paused, Completed, Archived }
public sealed record RunbookSchedulingRecord(Guid Id, string Name, string Owner, RunbookSchedulingState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record RunbookSchedulingCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record RunbookSchedulingEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record RunbookSchedulingQuery(string? SearchText, RunbookSchedulingState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record RunbookSchedulingPage(IReadOnlyList<RunbookSchedulingRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record RunbookSchedulingMutation(bool Succeeded, string Code, string Message, RunbookSchedulingRecord? Record, RunbookSchedulingEvent? Event);
public interface IRunbookSchedulingRepository
{
    ValueTask<RunbookSchedulingRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<RunbookSchedulingPage> QueryAsync(RunbookSchedulingQuery query, CancellationToken cancellationToken);
    ValueTask<RunbookSchedulingMutation> SaveAsync(RunbookSchedulingRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface IRunbookSchedulingEventSink { ValueTask PublishAsync(RunbookSchedulingEvent domainEvent, CancellationToken cancellationToken); }