namespace AtlasOps.Modules.Incidents.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum AlertTriageState { Draft, Active, Paused, Completed, Archived }
public sealed record AlertTriageRecord(Guid Id, string Name, string Owner, AlertTriageState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record AlertTriageCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record AlertTriageEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record AlertTriageQuery(string? SearchText, AlertTriageState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record AlertTriagePage(IReadOnlyList<AlertTriageRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record AlertTriageMutation(bool Succeeded, string Code, string Message, AlertTriageRecord? Record, AlertTriageEvent? Event);
public interface IAlertTriageRepository
{
    ValueTask<AlertTriageRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<AlertTriagePage> QueryAsync(AlertTriageQuery query, CancellationToken cancellationToken);
    ValueTask<AlertTriageMutation> SaveAsync(AlertTriageRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface IAlertTriageEventSink { ValueTask PublishAsync(AlertTriageEvent domainEvent, CancellationToken cancellationToken); }