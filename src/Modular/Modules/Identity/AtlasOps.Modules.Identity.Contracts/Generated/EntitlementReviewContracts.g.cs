namespace AtlasOps.Modules.Identity.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum EntitlementReviewState { Draft, Active, Paused, Completed, Archived }
public sealed record EntitlementReviewRecord(Guid Id, string Name, string Owner, EntitlementReviewState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record EntitlementReviewCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record EntitlementReviewEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record EntitlementReviewQuery(string? SearchText, EntitlementReviewState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record EntitlementReviewPage(IReadOnlyList<EntitlementReviewRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record EntitlementReviewMutation(bool Succeeded, string Code, string Message, EntitlementReviewRecord? Record, EntitlementReviewEvent? Event);
public interface IEntitlementReviewRepository
{
    ValueTask<EntitlementReviewRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<EntitlementReviewPage> QueryAsync(EntitlementReviewQuery query, CancellationToken cancellationToken);
    ValueTask<EntitlementReviewMutation> SaveAsync(EntitlementReviewRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface IEntitlementReviewEventSink { ValueTask PublishAsync(EntitlementReviewEvent domainEvent, CancellationToken cancellationToken); }