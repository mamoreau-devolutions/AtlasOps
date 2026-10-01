namespace AtlasOps.Modules.Deployments.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum RollbackMonitoringState { Draft, Active, Paused, Completed, Archived }
public sealed record RollbackMonitoringRecord(Guid Id, string Name, string Owner, RollbackMonitoringState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record RollbackMonitoringCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record RollbackMonitoringEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record RollbackMonitoringQuery(string? SearchText, RollbackMonitoringState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record RollbackMonitoringPage(IReadOnlyList<RollbackMonitoringRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record RollbackMonitoringMutation(bool Succeeded, string Code, string Message, RollbackMonitoringRecord? Record, RollbackMonitoringEvent? Event);
public interface IRollbackMonitoringRepository
{
    ValueTask<RollbackMonitoringRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<RollbackMonitoringPage> QueryAsync(RollbackMonitoringQuery query, CancellationToken cancellationToken);
    ValueTask<RollbackMonitoringMutation> SaveAsync(RollbackMonitoringRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface IRollbackMonitoringEventSink { ValueTask PublishAsync(RollbackMonitoringEvent domainEvent, CancellationToken cancellationToken); }