namespace AtlasOps.Modules.Observability.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum DashboardAggregationState { Draft, Active, Paused, Completed, Archived }
public sealed record DashboardAggregationRecord(Guid Id, string Name, string Owner, DashboardAggregationState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record DashboardAggregationCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record DashboardAggregationEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record DashboardAggregationQuery(string? SearchText, DashboardAggregationState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record DashboardAggregationPage(IReadOnlyList<DashboardAggregationRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record DashboardAggregationMutation(bool Succeeded, string Code, string Message, DashboardAggregationRecord? Record, DashboardAggregationEvent? Event);
public interface IDashboardAggregationRepository
{
    ValueTask<DashboardAggregationRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<DashboardAggregationPage> QueryAsync(DashboardAggregationQuery query, CancellationToken cancellationToken);
    ValueTask<DashboardAggregationMutation> SaveAsync(DashboardAggregationRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface IDashboardAggregationEventSink { ValueTask PublishAsync(DashboardAggregationEvent domainEvent, CancellationToken cancellationToken); }