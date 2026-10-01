namespace AtlasOps.Modules.Observability.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum MetricRetentionState { Draft, Active, Paused, Completed, Archived }
public sealed record MetricRetentionRecord(Guid Id, string Name, string Owner, MetricRetentionState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record MetricRetentionCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record MetricRetentionEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record MetricRetentionQuery(string? SearchText, MetricRetentionState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record MetricRetentionPage(IReadOnlyList<MetricRetentionRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record MetricRetentionMutation(bool Succeeded, string Code, string Message, MetricRetentionRecord? Record, MetricRetentionEvent? Event);
public interface IMetricRetentionRepository
{
    ValueTask<MetricRetentionRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<MetricRetentionPage> QueryAsync(MetricRetentionQuery query, CancellationToken cancellationToken);
    ValueTask<MetricRetentionMutation> SaveAsync(MetricRetentionRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface IMetricRetentionEventSink { ValueTask PublishAsync(MetricRetentionEvent domainEvent, CancellationToken cancellationToken); }