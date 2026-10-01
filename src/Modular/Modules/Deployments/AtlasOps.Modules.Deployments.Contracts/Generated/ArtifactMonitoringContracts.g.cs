namespace AtlasOps.Modules.Deployments.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum ArtifactMonitoringState { Draft, Active, Paused, Completed, Archived }
public sealed record ArtifactMonitoringRecord(Guid Id, string Name, string Owner, ArtifactMonitoringState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record ArtifactMonitoringCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record ArtifactMonitoringEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record ArtifactMonitoringQuery(string? SearchText, ArtifactMonitoringState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record ArtifactMonitoringPage(IReadOnlyList<ArtifactMonitoringRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record ArtifactMonitoringMutation(bool Succeeded, string Code, string Message, ArtifactMonitoringRecord? Record, ArtifactMonitoringEvent? Event);
public interface IArtifactMonitoringRepository
{
    ValueTask<ArtifactMonitoringRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<ArtifactMonitoringPage> QueryAsync(ArtifactMonitoringQuery query, CancellationToken cancellationToken);
    ValueTask<ArtifactMonitoringMutation> SaveAsync(ArtifactMonitoringRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface IArtifactMonitoringEventSink { ValueTask PublishAsync(ArtifactMonitoringEvent domainEvent, CancellationToken cancellationToken); }