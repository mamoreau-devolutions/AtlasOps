namespace AtlasOps.Modules.Deployments.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum RolloutPromotionState { Draft, Active, Paused, Completed, Archived }
public sealed record RolloutPromotionRecord(Guid Id, string Name, string Owner, RolloutPromotionState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record RolloutPromotionCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record RolloutPromotionEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record RolloutPromotionQuery(string? SearchText, RolloutPromotionState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record RolloutPromotionPage(IReadOnlyList<RolloutPromotionRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record RolloutPromotionMutation(bool Succeeded, string Code, string Message, RolloutPromotionRecord? Record, RolloutPromotionEvent? Event);
public interface IRolloutPromotionRepository
{
    ValueTask<RolloutPromotionRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<RolloutPromotionPage> QueryAsync(RolloutPromotionQuery query, CancellationToken cancellationToken);
    ValueTask<RolloutPromotionMutation> SaveAsync(RolloutPromotionRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface IRolloutPromotionEventSink { ValueTask PublishAsync(RolloutPromotionEvent domainEvent, CancellationToken cancellationToken); }