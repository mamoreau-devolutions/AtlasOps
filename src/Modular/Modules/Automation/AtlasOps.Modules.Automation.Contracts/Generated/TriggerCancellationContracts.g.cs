namespace AtlasOps.Modules.Automation.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum TriggerCancellationState { Draft, Active, Paused, Completed, Archived }
public sealed record TriggerCancellationRecord(Guid Id, string Name, string Owner, TriggerCancellationState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record TriggerCancellationCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record TriggerCancellationEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record TriggerCancellationQuery(string? SearchText, TriggerCancellationState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record TriggerCancellationPage(IReadOnlyList<TriggerCancellationRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record TriggerCancellationMutation(bool Succeeded, string Code, string Message, TriggerCancellationRecord? Record, TriggerCancellationEvent? Event);
public interface ITriggerCancellationRepository
{
    ValueTask<TriggerCancellationRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<TriggerCancellationPage> QueryAsync(TriggerCancellationQuery query, CancellationToken cancellationToken);
    ValueTask<TriggerCancellationMutation> SaveAsync(TriggerCancellationRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface ITriggerCancellationEventSink { ValueTask PublishAsync(TriggerCancellationEvent domainEvent, CancellationToken cancellationToken); }