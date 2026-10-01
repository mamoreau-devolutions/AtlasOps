namespace AtlasOps.Modules.Observability.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum TraceAggregationState { Draft, Active, Paused, Completed, Archived }
public sealed record TraceAggregationRecord(Guid Id, string Name, string Owner, TraceAggregationState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record TraceAggregationCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record TraceAggregationEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record TraceAggregationQuery(string? SearchText, TraceAggregationState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record TraceAggregationPage(IReadOnlyList<TraceAggregationRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record TraceAggregationMutation(bool Succeeded, string Code, string Message, TraceAggregationRecord? Record, TraceAggregationEvent? Event);
public interface ITraceAggregationRepository
{
    ValueTask<TraceAggregationRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<TraceAggregationPage> QueryAsync(TraceAggregationQuery query, CancellationToken cancellationToken);
    ValueTask<TraceAggregationMutation> SaveAsync(TraceAggregationRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface ITraceAggregationEventSink { ValueTask PublishAsync(TraceAggregationEvent domainEvent, CancellationToken cancellationToken); }