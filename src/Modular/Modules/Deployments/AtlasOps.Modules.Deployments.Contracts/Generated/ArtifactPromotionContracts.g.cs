namespace AtlasOps.Modules.Deployments.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum ArtifactPromotionState { Draft, Active, Paused, Completed, Archived }
public sealed record ArtifactPromotionRecord(Guid Id, string Name, string Owner, ArtifactPromotionState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record ArtifactPromotionCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record ArtifactPromotionEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record ArtifactPromotionQuery(string? SearchText, ArtifactPromotionState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record ArtifactPromotionPage(IReadOnlyList<ArtifactPromotionRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record ArtifactPromotionMutation(bool Succeeded, string Code, string Message, ArtifactPromotionRecord? Record, ArtifactPromotionEvent? Event);
public interface IArtifactPromotionRepository
{
    ValueTask<ArtifactPromotionRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<ArtifactPromotionPage> QueryAsync(ArtifactPromotionQuery query, CancellationToken cancellationToken);
    ValueTask<ArtifactPromotionMutation> SaveAsync(ArtifactPromotionRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface IArtifactPromotionEventSink { ValueTask PublishAsync(ArtifactPromotionEvent domainEvent, CancellationToken cancellationToken); }