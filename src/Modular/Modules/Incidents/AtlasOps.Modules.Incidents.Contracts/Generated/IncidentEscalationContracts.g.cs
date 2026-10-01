namespace AtlasOps.Modules.Incidents.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum IncidentEscalationState { Draft, Active, Paused, Completed, Archived }
public sealed record IncidentEscalationRecord(Guid Id, string Name, string Owner, IncidentEscalationState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record IncidentEscalationCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record IncidentEscalationEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record IncidentEscalationQuery(string? SearchText, IncidentEscalationState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record IncidentEscalationPage(IReadOnlyList<IncidentEscalationRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record IncidentEscalationMutation(bool Succeeded, string Code, string Message, IncidentEscalationRecord? Record, IncidentEscalationEvent? Event);
public interface IIncidentEscalationRepository
{
    ValueTask<IncidentEscalationRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<IncidentEscalationPage> QueryAsync(IncidentEscalationQuery query, CancellationToken cancellationToken);
    ValueTask<IncidentEscalationMutation> SaveAsync(IncidentEscalationRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface IIncidentEscalationEventSink { ValueTask PublishAsync(IncidentEscalationEvent domainEvent, CancellationToken cancellationToken); }