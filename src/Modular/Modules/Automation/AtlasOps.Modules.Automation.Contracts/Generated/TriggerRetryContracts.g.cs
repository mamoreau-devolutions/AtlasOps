namespace AtlasOps.Modules.Automation.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum TriggerRetryState { Draft, Active, Paused, Completed, Archived }
public sealed record TriggerRetryRecord(Guid Id, string Name, string Owner, TriggerRetryState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record TriggerRetryCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record TriggerRetryEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record TriggerRetryQuery(string? SearchText, TriggerRetryState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record TriggerRetryPage(IReadOnlyList<TriggerRetryRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record TriggerRetryMutation(bool Succeeded, string Code, string Message, TriggerRetryRecord? Record, TriggerRetryEvent? Event);
public interface ITriggerRetryRepository
{
    ValueTask<TriggerRetryRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<TriggerRetryPage> QueryAsync(TriggerRetryQuery query, CancellationToken cancellationToken);
    ValueTask<TriggerRetryMutation> SaveAsync(TriggerRetryRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface ITriggerRetryEventSink { ValueTask PublishAsync(TriggerRetryEvent domainEvent, CancellationToken cancellationToken); }