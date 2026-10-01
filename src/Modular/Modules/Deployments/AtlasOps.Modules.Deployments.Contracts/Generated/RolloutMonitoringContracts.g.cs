namespace AtlasOps.Modules.Deployments.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum RolloutMonitoringState { Draft, Active, Paused, Completed, Archived }
public sealed record RolloutMonitoringRecord(Guid Id, string Name, string Owner, RolloutMonitoringState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record RolloutMonitoringCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record RolloutMonitoringEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record RolloutMonitoringQuery(string? SearchText, RolloutMonitoringState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record RolloutMonitoringPage(IReadOnlyList<RolloutMonitoringRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record RolloutMonitoringMutation(bool Succeeded, string Code, string Message, RolloutMonitoringRecord? Record, RolloutMonitoringEvent? Event);
public interface IRolloutMonitoringRepository
{
    ValueTask<RolloutMonitoringRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<RolloutMonitoringPage> QueryAsync(RolloutMonitoringQuery query, CancellationToken cancellationToken);
    ValueTask<RolloutMonitoringMutation> SaveAsync(RolloutMonitoringRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface IRolloutMonitoringEventSink { ValueTask PublishAsync(RolloutMonitoringEvent domainEvent, CancellationToken cancellationToken); }