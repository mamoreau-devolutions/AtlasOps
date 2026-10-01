namespace AtlasOps.Modules.Deployments.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum ArtifactPlanningState { Draft, Active, Paused, Completed, Archived }
public sealed record ArtifactPlanningRecord(Guid Id, string Name, string Owner, ArtifactPlanningState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record ArtifactPlanningCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record ArtifactPlanningEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record ArtifactPlanningQuery(string? SearchText, ArtifactPlanningState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record ArtifactPlanningPage(IReadOnlyList<ArtifactPlanningRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record ArtifactPlanningMutation(bool Succeeded, string Code, string Message, ArtifactPlanningRecord? Record, ArtifactPlanningEvent? Event);
public interface IArtifactPlanningRepository
{
    ValueTask<ArtifactPlanningRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<ArtifactPlanningPage> QueryAsync(ArtifactPlanningQuery query, CancellationToken cancellationToken);
    ValueTask<ArtifactPlanningMutation> SaveAsync(ArtifactPlanningRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface IArtifactPlanningEventSink { ValueTask PublishAsync(ArtifactPlanningEvent domainEvent, CancellationToken cancellationToken); }