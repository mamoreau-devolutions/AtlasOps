namespace AtlasOps.Modules.Observability.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum DashboardThresholdState { Draft, Active, Paused, Completed, Archived }
public sealed record DashboardThresholdRecord(Guid Id, string Name, string Owner, DashboardThresholdState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record DashboardThresholdCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record DashboardThresholdEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record DashboardThresholdQuery(string? SearchText, DashboardThresholdState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record DashboardThresholdPage(IReadOnlyList<DashboardThresholdRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record DashboardThresholdMutation(bool Succeeded, string Code, string Message, DashboardThresholdRecord? Record, DashboardThresholdEvent? Event);
public interface IDashboardThresholdRepository
{
    ValueTask<DashboardThresholdRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<DashboardThresholdPage> QueryAsync(DashboardThresholdQuery query, CancellationToken cancellationToken);
    ValueTask<DashboardThresholdMutation> SaveAsync(DashboardThresholdRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface IDashboardThresholdEventSink { ValueTask PublishAsync(DashboardThresholdEvent domainEvent, CancellationToken cancellationToken); }