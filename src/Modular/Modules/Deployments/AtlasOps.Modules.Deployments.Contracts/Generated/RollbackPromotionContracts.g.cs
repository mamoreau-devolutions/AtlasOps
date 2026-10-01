namespace AtlasOps.Modules.Deployments.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum RollbackPromotionState { Draft, Active, Paused, Completed, Archived }
public sealed record RollbackPromotionRecord(Guid Id, string Name, string Owner, RollbackPromotionState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record RollbackPromotionCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record RollbackPromotionEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record RollbackPromotionQuery(string? SearchText, RollbackPromotionState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record RollbackPromotionPage(IReadOnlyList<RollbackPromotionRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record RollbackPromotionMutation(bool Succeeded, string Code, string Message, RollbackPromotionRecord? Record, RollbackPromotionEvent? Event);
public interface IRollbackPromotionRepository
{
    ValueTask<RollbackPromotionRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<RollbackPromotionPage> QueryAsync(RollbackPromotionQuery query, CancellationToken cancellationToken);
    ValueTask<RollbackPromotionMutation> SaveAsync(RollbackPromotionRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface IRollbackPromotionEventSink { ValueTask PublishAsync(RollbackPromotionEvent domainEvent, CancellationToken cancellationToken); }