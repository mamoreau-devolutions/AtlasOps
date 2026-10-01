namespace AtlasOps.Modules.Observability.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum MetricCollectionState { Draft, Active, Paused, Completed, Archived }
public sealed record MetricCollectionRecord(Guid Id, string Name, string Owner, MetricCollectionState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record MetricCollectionCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record MetricCollectionEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record MetricCollectionQuery(string? SearchText, MetricCollectionState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record MetricCollectionPage(IReadOnlyList<MetricCollectionRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record MetricCollectionMutation(bool Succeeded, string Code, string Message, MetricCollectionRecord? Record, MetricCollectionEvent? Event);
public interface IMetricCollectionRepository
{
    ValueTask<MetricCollectionRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<MetricCollectionPage> QueryAsync(MetricCollectionQuery query, CancellationToken cancellationToken);
    ValueTask<MetricCollectionMutation> SaveAsync(MetricCollectionRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface IMetricCollectionEventSink { ValueTask PublishAsync(MetricCollectionEvent domainEvent, CancellationToken cancellationToken); }