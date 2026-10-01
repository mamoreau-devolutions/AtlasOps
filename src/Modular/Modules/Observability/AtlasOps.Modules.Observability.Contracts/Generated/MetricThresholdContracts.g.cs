namespace AtlasOps.Modules.Observability.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum MetricThresholdState { Draft, Active, Paused, Completed, Archived }
public sealed record MetricThresholdRecord(Guid Id, string Name, string Owner, MetricThresholdState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record MetricThresholdCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record MetricThresholdEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record MetricThresholdQuery(string? SearchText, MetricThresholdState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record MetricThresholdPage(IReadOnlyList<MetricThresholdRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record MetricThresholdMutation(bool Succeeded, string Code, string Message, MetricThresholdRecord? Record, MetricThresholdEvent? Event);
public interface IMetricThresholdRepository
{
    ValueTask<MetricThresholdRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<MetricThresholdPage> QueryAsync(MetricThresholdQuery query, CancellationToken cancellationToken);
    ValueTask<MetricThresholdMutation> SaveAsync(MetricThresholdRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface IMetricThresholdEventSink { ValueTask PublishAsync(MetricThresholdEvent domainEvent, CancellationToken cancellationToken); }