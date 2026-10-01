namespace AtlasOps.Modules.Deployments.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum EnvironmentPromotionState { Draft, Active, Paused, Completed, Archived }
public sealed record EnvironmentPromotionRecord(Guid Id, string Name, string Owner, EnvironmentPromotionState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record EnvironmentPromotionCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record EnvironmentPromotionEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record EnvironmentPromotionQuery(string? SearchText, EnvironmentPromotionState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record EnvironmentPromotionPage(IReadOnlyList<EnvironmentPromotionRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record EnvironmentPromotionMutation(bool Succeeded, string Code, string Message, EnvironmentPromotionRecord? Record, EnvironmentPromotionEvent? Event);
public interface IEnvironmentPromotionRepository
{
    ValueTask<EnvironmentPromotionRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<EnvironmentPromotionPage> QueryAsync(EnvironmentPromotionQuery query, CancellationToken cancellationToken);
    ValueTask<EnvironmentPromotionMutation> SaveAsync(EnvironmentPromotionRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface IEnvironmentPromotionEventSink { ValueTask PublishAsync(EnvironmentPromotionEvent domainEvent, CancellationToken cancellationToken); }