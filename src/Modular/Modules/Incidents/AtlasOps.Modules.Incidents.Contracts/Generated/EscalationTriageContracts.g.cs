namespace AtlasOps.Modules.Incidents.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum EscalationTriageState { Draft, Active, Paused, Completed, Archived }
public sealed record EscalationTriageRecord(Guid Id, string Name, string Owner, EscalationTriageState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record EscalationTriageCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record EscalationTriageEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record EscalationTriageQuery(string? SearchText, EscalationTriageState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record EscalationTriagePage(IReadOnlyList<EscalationTriageRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record EscalationTriageMutation(bool Succeeded, string Code, string Message, EscalationTriageRecord? Record, EscalationTriageEvent? Event);
public interface IEscalationTriageRepository
{
    ValueTask<EscalationTriageRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<EscalationTriagePage> QueryAsync(EscalationTriageQuery query, CancellationToken cancellationToken);
    ValueTask<EscalationTriageMutation> SaveAsync(EscalationTriageRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface IEscalationTriageEventSink { ValueTask PublishAsync(EscalationTriageEvent domainEvent, CancellationToken cancellationToken); }