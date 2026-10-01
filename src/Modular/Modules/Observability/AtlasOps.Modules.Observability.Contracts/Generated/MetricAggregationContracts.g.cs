namespace AtlasOps.Modules.Observability.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum MetricAggregationState { Draft, Active, Paused, Completed, Archived }
public sealed record MetricAggregationRecord(Guid Id, string Name, string Owner, MetricAggregationState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record MetricAggregationCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record MetricAggregationEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record MetricAggregationQuery(string? SearchText, MetricAggregationState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record MetricAggregationPage(IReadOnlyList<MetricAggregationRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record MetricAggregationMutation(bool Succeeded, string Code, string Message, MetricAggregationRecord? Record, MetricAggregationEvent? Event);
public interface IMetricAggregationRepository
{
    ValueTask<MetricAggregationRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<MetricAggregationPage> QueryAsync(MetricAggregationQuery query, CancellationToken cancellationToken);
    ValueTask<MetricAggregationMutation> SaveAsync(MetricAggregationRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface IMetricAggregationEventSink { ValueTask PublishAsync(MetricAggregationEvent domainEvent, CancellationToken cancellationToken); }