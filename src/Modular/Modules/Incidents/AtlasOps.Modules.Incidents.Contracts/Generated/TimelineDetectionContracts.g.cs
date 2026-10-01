namespace AtlasOps.Modules.Incidents.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum TimelineDetectionState { Draft, Active, Paused, Completed, Archived }
public sealed record TimelineDetectionRecord(Guid Id, string Name, string Owner, TimelineDetectionState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record TimelineDetectionCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record TimelineDetectionEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record TimelineDetectionQuery(string? SearchText, TimelineDetectionState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record TimelineDetectionPage(IReadOnlyList<TimelineDetectionRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record TimelineDetectionMutation(bool Succeeded, string Code, string Message, TimelineDetectionRecord? Record, TimelineDetectionEvent? Event);
public interface ITimelineDetectionRepository
{
    ValueTask<TimelineDetectionRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<TimelineDetectionPage> QueryAsync(TimelineDetectionQuery query, CancellationToken cancellationToken);
    ValueTask<TimelineDetectionMutation> SaveAsync(TimelineDetectionRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface ITimelineDetectionEventSink { ValueTask PublishAsync(TimelineDetectionEvent domainEvent, CancellationToken cancellationToken); }