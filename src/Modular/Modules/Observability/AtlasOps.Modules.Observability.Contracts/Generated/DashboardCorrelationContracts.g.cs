namespace AtlasOps.Modules.Observability.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum DashboardCorrelationState { Draft, Active, Paused, Completed, Archived }
public sealed record DashboardCorrelationRecord(Guid Id, string Name, string Owner, DashboardCorrelationState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record DashboardCorrelationCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record DashboardCorrelationEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record DashboardCorrelationQuery(string? SearchText, DashboardCorrelationState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record DashboardCorrelationPage(IReadOnlyList<DashboardCorrelationRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record DashboardCorrelationMutation(bool Succeeded, string Code, string Message, DashboardCorrelationRecord? Record, DashboardCorrelationEvent? Event);
public interface IDashboardCorrelationRepository
{
    ValueTask<DashboardCorrelationRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<DashboardCorrelationPage> QueryAsync(DashboardCorrelationQuery query, CancellationToken cancellationToken);
    ValueTask<DashboardCorrelationMutation> SaveAsync(DashboardCorrelationRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface IDashboardCorrelationEventSink { ValueTask PublishAsync(DashboardCorrelationEvent domainEvent, CancellationToken cancellationToken); }