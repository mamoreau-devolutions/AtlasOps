namespace AtlasOps.Modules.Deployments.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum EnvironmentMonitoringState { Draft, Active, Paused, Completed, Archived }
public sealed record EnvironmentMonitoringRecord(Guid Id, string Name, string Owner, EnvironmentMonitoringState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record EnvironmentMonitoringCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record EnvironmentMonitoringEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record EnvironmentMonitoringQuery(string? SearchText, EnvironmentMonitoringState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record EnvironmentMonitoringPage(IReadOnlyList<EnvironmentMonitoringRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record EnvironmentMonitoringMutation(bool Succeeded, string Code, string Message, EnvironmentMonitoringRecord? Record, EnvironmentMonitoringEvent? Event);
public interface IEnvironmentMonitoringRepository
{
    ValueTask<EnvironmentMonitoringRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<EnvironmentMonitoringPage> QueryAsync(EnvironmentMonitoringQuery query, CancellationToken cancellationToken);
    ValueTask<EnvironmentMonitoringMutation> SaveAsync(EnvironmentMonitoringRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface IEnvironmentMonitoringEventSink { ValueTask PublishAsync(EnvironmentMonitoringEvent domainEvent, CancellationToken cancellationToken); }