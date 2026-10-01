namespace AtlasOps.Modules.Incidents.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum TimelineTriageState { Draft, Active, Paused, Completed, Archived }
public sealed record TimelineTriageRecord(Guid Id, string Name, string Owner, TimelineTriageState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record TimelineTriageCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record TimelineTriageEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record TimelineTriageQuery(string? SearchText, TimelineTriageState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record TimelineTriagePage(IReadOnlyList<TimelineTriageRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record TimelineTriageMutation(bool Succeeded, string Code, string Message, TimelineTriageRecord? Record, TimelineTriageEvent? Event);
public interface ITimelineTriageRepository
{
    ValueTask<TimelineTriageRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<TimelineTriagePage> QueryAsync(TimelineTriageQuery query, CancellationToken cancellationToken);
    ValueTask<TimelineTriageMutation> SaveAsync(TimelineTriageRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface ITimelineTriageEventSink { ValueTask PublishAsync(TimelineTriageEvent domainEvent, CancellationToken cancellationToken); }