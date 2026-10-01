namespace AtlasOps.Modules.Incidents.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum IncidentDetectionState { Draft, Active, Paused, Completed, Archived }
public sealed record IncidentDetectionRecord(Guid Id, string Name, string Owner, IncidentDetectionState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record IncidentDetectionCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record IncidentDetectionEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record IncidentDetectionQuery(string? SearchText, IncidentDetectionState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record IncidentDetectionPage(IReadOnlyList<IncidentDetectionRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record IncidentDetectionMutation(bool Succeeded, string Code, string Message, IncidentDetectionRecord? Record, IncidentDetectionEvent? Event);
public interface IIncidentDetectionRepository
{
    ValueTask<IncidentDetectionRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<IncidentDetectionPage> QueryAsync(IncidentDetectionQuery query, CancellationToken cancellationToken);
    ValueTask<IncidentDetectionMutation> SaveAsync(IncidentDetectionRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface IIncidentDetectionEventSink { ValueTask PublishAsync(IncidentDetectionEvent domainEvent, CancellationToken cancellationToken); }