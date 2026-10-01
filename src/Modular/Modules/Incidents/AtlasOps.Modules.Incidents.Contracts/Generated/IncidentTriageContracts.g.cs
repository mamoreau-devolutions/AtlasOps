namespace AtlasOps.Modules.Incidents.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum IncidentTriageState { Draft, Active, Paused, Completed, Archived }
public sealed record IncidentTriageRecord(Guid Id, string Name, string Owner, IncidentTriageState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record IncidentTriageCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record IncidentTriageEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record IncidentTriageQuery(string? SearchText, IncidentTriageState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record IncidentTriagePage(IReadOnlyList<IncidentTriageRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record IncidentTriageMutation(bool Succeeded, string Code, string Message, IncidentTriageRecord? Record, IncidentTriageEvent? Event);
public interface IIncidentTriageRepository
{
    ValueTask<IncidentTriageRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<IncidentTriagePage> QueryAsync(IncidentTriageQuery query, CancellationToken cancellationToken);
    ValueTask<IncidentTriageMutation> SaveAsync(IncidentTriageRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface IIncidentTriageEventSink { ValueTask PublishAsync(IncidentTriageEvent domainEvent, CancellationToken cancellationToken); }