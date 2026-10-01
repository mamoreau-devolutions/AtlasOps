namespace AtlasOps.Modules.Observability.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum MetricCorrelationState { Draft, Active, Paused, Completed, Archived }
public sealed record MetricCorrelationRecord(Guid Id, string Name, string Owner, MetricCorrelationState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record MetricCorrelationCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record MetricCorrelationEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record MetricCorrelationQuery(string? SearchText, MetricCorrelationState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record MetricCorrelationPage(IReadOnlyList<MetricCorrelationRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record MetricCorrelationMutation(bool Succeeded, string Code, string Message, MetricCorrelationRecord? Record, MetricCorrelationEvent? Event);
public interface IMetricCorrelationRepository
{
    ValueTask<MetricCorrelationRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<MetricCorrelationPage> QueryAsync(MetricCorrelationQuery query, CancellationToken cancellationToken);
    ValueTask<MetricCorrelationMutation> SaveAsync(MetricCorrelationRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface IMetricCorrelationEventSink { ValueTask PublishAsync(MetricCorrelationEvent domainEvent, CancellationToken cancellationToken); }