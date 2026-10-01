namespace AtlasOps.Modules.Observability.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum ObjectiveAggregationState { Draft, Active, Paused, Completed, Archived }
public sealed record ObjectiveAggregationRecord(Guid Id, string Name, string Owner, ObjectiveAggregationState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record ObjectiveAggregationCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record ObjectiveAggregationEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record ObjectiveAggregationQuery(string? SearchText, ObjectiveAggregationState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record ObjectiveAggregationPage(IReadOnlyList<ObjectiveAggregationRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record ObjectiveAggregationMutation(bool Succeeded, string Code, string Message, ObjectiveAggregationRecord? Record, ObjectiveAggregationEvent? Event);
public interface IObjectiveAggregationRepository
{
    ValueTask<ObjectiveAggregationRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<ObjectiveAggregationPage> QueryAsync(ObjectiveAggregationQuery query, CancellationToken cancellationToken);
    ValueTask<ObjectiveAggregationMutation> SaveAsync(ObjectiveAggregationRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface IObjectiveAggregationEventSink { ValueTask PublishAsync(ObjectiveAggregationEvent domainEvent, CancellationToken cancellationToken); }