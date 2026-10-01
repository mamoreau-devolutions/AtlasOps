namespace AtlasOps.Modules.Incidents.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum TimelineAssignmentState { Draft, Active, Paused, Completed, Archived }
public sealed record TimelineAssignmentRecord(Guid Id, string Name, string Owner, TimelineAssignmentState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record TimelineAssignmentCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record TimelineAssignmentEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record TimelineAssignmentQuery(string? SearchText, TimelineAssignmentState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record TimelineAssignmentPage(IReadOnlyList<TimelineAssignmentRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record TimelineAssignmentMutation(bool Succeeded, string Code, string Message, TimelineAssignmentRecord? Record, TimelineAssignmentEvent? Event);
public interface ITimelineAssignmentRepository
{
    ValueTask<TimelineAssignmentRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<TimelineAssignmentPage> QueryAsync(TimelineAssignmentQuery query, CancellationToken cancellationToken);
    ValueTask<TimelineAssignmentMutation> SaveAsync(TimelineAssignmentRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface ITimelineAssignmentEventSink { ValueTask PublishAsync(TimelineAssignmentEvent domainEvent, CancellationToken cancellationToken); }