namespace AtlasOps.Modules.Incidents.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum TimelineResolutionState { Draft, Active, Paused, Completed, Archived }
public sealed record TimelineResolutionRecord(Guid Id, string Name, string Owner, TimelineResolutionState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record TimelineResolutionCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record TimelineResolutionEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record TimelineResolutionQuery(string? SearchText, TimelineResolutionState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record TimelineResolutionPage(IReadOnlyList<TimelineResolutionRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record TimelineResolutionMutation(bool Succeeded, string Code, string Message, TimelineResolutionRecord? Record, TimelineResolutionEvent? Event);
public interface ITimelineResolutionRepository
{
    ValueTask<TimelineResolutionRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<TimelineResolutionPage> QueryAsync(TimelineResolutionQuery query, CancellationToken cancellationToken);
    ValueTask<TimelineResolutionMutation> SaveAsync(TimelineResolutionRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface ITimelineResolutionEventSink { ValueTask PublishAsync(TimelineResolutionEvent domainEvent, CancellationToken cancellationToken); }