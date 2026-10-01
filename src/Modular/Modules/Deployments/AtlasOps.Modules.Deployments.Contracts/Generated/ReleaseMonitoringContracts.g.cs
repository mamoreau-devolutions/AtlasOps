namespace AtlasOps.Modules.Deployments.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum ReleaseMonitoringState { Draft, Active, Paused, Completed, Archived }
public sealed record ReleaseMonitoringRecord(Guid Id, string Name, string Owner, ReleaseMonitoringState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record ReleaseMonitoringCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record ReleaseMonitoringEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record ReleaseMonitoringQuery(string? SearchText, ReleaseMonitoringState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record ReleaseMonitoringPage(IReadOnlyList<ReleaseMonitoringRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record ReleaseMonitoringMutation(bool Succeeded, string Code, string Message, ReleaseMonitoringRecord? Record, ReleaseMonitoringEvent? Event);
public interface IReleaseMonitoringRepository
{
    ValueTask<ReleaseMonitoringRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<ReleaseMonitoringPage> QueryAsync(ReleaseMonitoringQuery query, CancellationToken cancellationToken);
    ValueTask<ReleaseMonitoringMutation> SaveAsync(ReleaseMonitoringRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface IReleaseMonitoringEventSink { ValueTask PublishAsync(ReleaseMonitoringEvent domainEvent, CancellationToken cancellationToken); }