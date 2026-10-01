namespace AtlasOps.Modules.Deployments.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum ArtifactRecoveryState { Draft, Active, Paused, Completed, Archived }
public sealed record ArtifactRecoveryRecord(Guid Id, string Name, string Owner, ArtifactRecoveryState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record ArtifactRecoveryCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record ArtifactRecoveryEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record ArtifactRecoveryQuery(string? SearchText, ArtifactRecoveryState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record ArtifactRecoveryPage(IReadOnlyList<ArtifactRecoveryRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record ArtifactRecoveryMutation(bool Succeeded, string Code, string Message, ArtifactRecoveryRecord? Record, ArtifactRecoveryEvent? Event);
public interface IArtifactRecoveryRepository
{
    ValueTask<ArtifactRecoveryRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<ArtifactRecoveryPage> QueryAsync(ArtifactRecoveryQuery query, CancellationToken cancellationToken);
    ValueTask<ArtifactRecoveryMutation> SaveAsync(ArtifactRecoveryRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface IArtifactRecoveryEventSink { ValueTask PublishAsync(ArtifactRecoveryEvent domainEvent, CancellationToken cancellationToken); }