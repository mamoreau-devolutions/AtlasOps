namespace AtlasOps.Modules.Incidents.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum EscalationEscalationState { Draft, Active, Paused, Completed, Archived }
public sealed record EscalationEscalationRecord(Guid Id, string Name, string Owner, EscalationEscalationState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record EscalationEscalationCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record EscalationEscalationEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record EscalationEscalationQuery(string? SearchText, EscalationEscalationState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record EscalationEscalationPage(IReadOnlyList<EscalationEscalationRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record EscalationEscalationMutation(bool Succeeded, string Code, string Message, EscalationEscalationRecord? Record, EscalationEscalationEvent? Event);
public interface IEscalationEscalationRepository
{
    ValueTask<EscalationEscalationRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<EscalationEscalationPage> QueryAsync(EscalationEscalationQuery query, CancellationToken cancellationToken);
    ValueTask<EscalationEscalationMutation> SaveAsync(EscalationEscalationRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface IEscalationEscalationEventSink { ValueTask PublishAsync(EscalationEscalationEvent domainEvent, CancellationToken cancellationToken); }