namespace AtlasOps.Modules.Deployments.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum ArtifactValidationState { Draft, Active, Paused, Completed, Archived }
public sealed record ArtifactValidationRecord(Guid Id, string Name, string Owner, ArtifactValidationState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record ArtifactValidationCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record ArtifactValidationEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record ArtifactValidationQuery(string? SearchText, ArtifactValidationState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record ArtifactValidationPage(IReadOnlyList<ArtifactValidationRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record ArtifactValidationMutation(bool Succeeded, string Code, string Message, ArtifactValidationRecord? Record, ArtifactValidationEvent? Event);
public interface IArtifactValidationRepository
{
    ValueTask<ArtifactValidationRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<ArtifactValidationPage> QueryAsync(ArtifactValidationQuery query, CancellationToken cancellationToken);
    ValueTask<ArtifactValidationMutation> SaveAsync(ArtifactValidationRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface IArtifactValidationEventSink { ValueTask PublishAsync(ArtifactValidationEvent domainEvent, CancellationToken cancellationToken); }