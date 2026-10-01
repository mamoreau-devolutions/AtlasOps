namespace AtlasOps.Modules.Observability.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum MetricReportingState { Draft, Active, Paused, Completed, Archived }
public sealed record MetricReportingRecord(Guid Id, string Name, string Owner, MetricReportingState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record MetricReportingCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record MetricReportingEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record MetricReportingQuery(string? SearchText, MetricReportingState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record MetricReportingPage(IReadOnlyList<MetricReportingRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record MetricReportingMutation(bool Succeeded, string Code, string Message, MetricReportingRecord? Record, MetricReportingEvent? Event);
public interface IMetricReportingRepository
{
    ValueTask<MetricReportingRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<MetricReportingPage> QueryAsync(MetricReportingQuery query, CancellationToken cancellationToken);
    ValueTask<MetricReportingMutation> SaveAsync(MetricReportingRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface IMetricReportingEventSink { ValueTask PublishAsync(MetricReportingEvent domainEvent, CancellationToken cancellationToken); }