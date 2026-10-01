namespace AtlasOps.Modules.Incidents.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum TimelineEscalationState { Draft, Active, Paused, Completed, Archived }
public sealed record TimelineEscalationRecord(Guid Id, string Name, string Owner, TimelineEscalationState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record TimelineEscalationCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record TimelineEscalationEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record TimelineEscalationQuery(string? SearchText, TimelineEscalationState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record TimelineEscalationPage(IReadOnlyList<TimelineEscalationRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record TimelineEscalationMutation(bool Succeeded, string Code, string Message, TimelineEscalationRecord? Record, TimelineEscalationEvent? Event);
public interface ITimelineEscalationRepository
{
    ValueTask<TimelineEscalationRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<TimelineEscalationPage> QueryAsync(TimelineEscalationQuery query, CancellationToken cancellationToken);
    ValueTask<TimelineEscalationMutation> SaveAsync(TimelineEscalationRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface ITimelineEscalationEventSink { ValueTask PublishAsync(TimelineEscalationEvent domainEvent, CancellationToken cancellationToken); }