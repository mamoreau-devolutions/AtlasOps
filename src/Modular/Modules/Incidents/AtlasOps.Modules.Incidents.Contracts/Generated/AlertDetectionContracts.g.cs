namespace AtlasOps.Modules.Incidents.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum AlertDetectionState { Draft, Active, Paused, Completed, Archived }
public sealed record AlertDetectionRecord(Guid Id, string Name, string Owner, AlertDetectionState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record AlertDetectionCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record AlertDetectionEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record AlertDetectionQuery(string? SearchText, AlertDetectionState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record AlertDetectionPage(IReadOnlyList<AlertDetectionRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record AlertDetectionMutation(bool Succeeded, string Code, string Message, AlertDetectionRecord? Record, AlertDetectionEvent? Event);
public interface IAlertDetectionRepository
{
    ValueTask<AlertDetectionRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<AlertDetectionPage> QueryAsync(AlertDetectionQuery query, CancellationToken cancellationToken);
    ValueTask<AlertDetectionMutation> SaveAsync(AlertDetectionRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface IAlertDetectionEventSink { ValueTask PublishAsync(AlertDetectionEvent domainEvent, CancellationToken cancellationToken); }