namespace AtlasOps.Modules.Incidents.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum RunbookResolutionState { Draft, Active, Paused, Completed, Archived }
public sealed record RunbookResolutionRecord(Guid Id, string Name, string Owner, RunbookResolutionState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record RunbookResolutionCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record RunbookResolutionEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record RunbookResolutionQuery(string? SearchText, RunbookResolutionState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record RunbookResolutionPage(IReadOnlyList<RunbookResolutionRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record RunbookResolutionMutation(bool Succeeded, string Code, string Message, RunbookResolutionRecord? Record, RunbookResolutionEvent? Event);
public interface IRunbookResolutionRepository
{
    ValueTask<RunbookResolutionRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<RunbookResolutionPage> QueryAsync(RunbookResolutionQuery query, CancellationToken cancellationToken);
    ValueTask<RunbookResolutionMutation> SaveAsync(RunbookResolutionRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface IRunbookResolutionEventSink { ValueTask PublishAsync(RunbookResolutionEvent domainEvent, CancellationToken cancellationToken); }