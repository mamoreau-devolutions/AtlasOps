namespace AtlasOps.Modules.Automation.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum TriggerSchedulingState { Draft, Active, Paused, Completed, Archived }
public sealed record TriggerSchedulingRecord(Guid Id, string Name, string Owner, TriggerSchedulingState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record TriggerSchedulingCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record TriggerSchedulingEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record TriggerSchedulingQuery(string? SearchText, TriggerSchedulingState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record TriggerSchedulingPage(IReadOnlyList<TriggerSchedulingRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record TriggerSchedulingMutation(bool Succeeded, string Code, string Message, TriggerSchedulingRecord? Record, TriggerSchedulingEvent? Event);
public interface ITriggerSchedulingRepository
{
    ValueTask<TriggerSchedulingRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<TriggerSchedulingPage> QueryAsync(TriggerSchedulingQuery query, CancellationToken cancellationToken);
    ValueTask<TriggerSchedulingMutation> SaveAsync(TriggerSchedulingRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface ITriggerSchedulingEventSink { ValueTask PublishAsync(TriggerSchedulingEvent domainEvent, CancellationToken cancellationToken); }