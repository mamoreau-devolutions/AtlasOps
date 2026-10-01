namespace AtlasOps.Modules.Incidents.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum TimelineReviewState { Draft, Active, Paused, Completed, Archived }
public sealed record TimelineReviewRecord(Guid Id, string Name, string Owner, TimelineReviewState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record TimelineReviewCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record TimelineReviewEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record TimelineReviewQuery(string? SearchText, TimelineReviewState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record TimelineReviewPage(IReadOnlyList<TimelineReviewRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record TimelineReviewMutation(bool Succeeded, string Code, string Message, TimelineReviewRecord? Record, TimelineReviewEvent? Event);
public interface ITimelineReviewRepository
{
    ValueTask<TimelineReviewRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<TimelineReviewPage> QueryAsync(TimelineReviewQuery query, CancellationToken cancellationToken);
    ValueTask<TimelineReviewMutation> SaveAsync(TimelineReviewRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface ITimelineReviewEventSink { ValueTask PublishAsync(TimelineReviewEvent domainEvent, CancellationToken cancellationToken); }