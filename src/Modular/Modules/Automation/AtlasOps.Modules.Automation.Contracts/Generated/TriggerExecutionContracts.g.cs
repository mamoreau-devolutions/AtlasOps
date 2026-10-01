namespace AtlasOps.Modules.Automation.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum TriggerExecutionState { Draft, Active, Paused, Completed, Archived }
public sealed record TriggerExecutionRecord(Guid Id, string Name, string Owner, TriggerExecutionState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record TriggerExecutionCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record TriggerExecutionEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record TriggerExecutionQuery(string? SearchText, TriggerExecutionState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record TriggerExecutionPage(IReadOnlyList<TriggerExecutionRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record TriggerExecutionMutation(bool Succeeded, string Code, string Message, TriggerExecutionRecord? Record, TriggerExecutionEvent? Event);
public interface ITriggerExecutionRepository
{
    ValueTask<TriggerExecutionRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<TriggerExecutionPage> QueryAsync(TriggerExecutionQuery query, CancellationToken cancellationToken);
    ValueTask<TriggerExecutionMutation> SaveAsync(TriggerExecutionRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface ITriggerExecutionEventSink { ValueTask PublishAsync(TriggerExecutionEvent domainEvent, CancellationToken cancellationToken); }