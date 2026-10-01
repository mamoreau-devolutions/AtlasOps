namespace AtlasOps.Modules.Deployments.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum ReleasePromotionState { Draft, Active, Paused, Completed, Archived }
public sealed record ReleasePromotionRecord(Guid Id, string Name, string Owner, ReleasePromotionState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record ReleasePromotionCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record ReleasePromotionEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record ReleasePromotionQuery(string? SearchText, ReleasePromotionState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record ReleasePromotionPage(IReadOnlyList<ReleasePromotionRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record ReleasePromotionMutation(bool Succeeded, string Code, string Message, ReleasePromotionRecord? Record, ReleasePromotionEvent? Event);
public interface IReleasePromotionRepository
{
    ValueTask<ReleasePromotionRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<ReleasePromotionPage> QueryAsync(ReleasePromotionQuery query, CancellationToken cancellationToken);
    ValueTask<ReleasePromotionMutation> SaveAsync(ReleasePromotionRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface IReleasePromotionEventSink { ValueTask PublishAsync(ReleasePromotionEvent domainEvent, CancellationToken cancellationToken); }