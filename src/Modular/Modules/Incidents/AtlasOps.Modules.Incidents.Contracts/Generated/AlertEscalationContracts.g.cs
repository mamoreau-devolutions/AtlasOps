namespace AtlasOps.Modules.Incidents.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum AlertEscalationState { Draft, Active, Paused, Completed, Archived }
public sealed record AlertEscalationRecord(Guid Id, string Name, string Owner, AlertEscalationState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record AlertEscalationCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record AlertEscalationEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record AlertEscalationQuery(string? SearchText, AlertEscalationState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record AlertEscalationPage(IReadOnlyList<AlertEscalationRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record AlertEscalationMutation(bool Succeeded, string Code, string Message, AlertEscalationRecord? Record, AlertEscalationEvent? Event);
public interface IAlertEscalationRepository
{
    ValueTask<AlertEscalationRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<AlertEscalationPage> QueryAsync(AlertEscalationQuery query, CancellationToken cancellationToken);
    ValueTask<AlertEscalationMutation> SaveAsync(AlertEscalationRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface IAlertEscalationEventSink { ValueTask PublishAsync(AlertEscalationEvent domainEvent, CancellationToken cancellationToken); }