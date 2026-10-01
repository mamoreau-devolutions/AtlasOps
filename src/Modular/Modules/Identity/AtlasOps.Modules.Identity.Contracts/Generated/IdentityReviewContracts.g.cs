namespace AtlasOps.Modules.Identity.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum IdentityReviewState { Draft, Active, Paused, Completed, Archived }
public sealed record IdentityReviewRecord(Guid Id, string Name, string Owner, IdentityReviewState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record IdentityReviewCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record IdentityReviewEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record IdentityReviewQuery(string? SearchText, IdentityReviewState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record IdentityReviewPage(IReadOnlyList<IdentityReviewRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record IdentityReviewMutation(bool Succeeded, string Code, string Message, IdentityReviewRecord? Record, IdentityReviewEvent? Event);
public interface IIdentityReviewRepository
{
    ValueTask<IdentityReviewRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<IdentityReviewPage> QueryAsync(IdentityReviewQuery query, CancellationToken cancellationToken);
    ValueTask<IdentityReviewMutation> SaveAsync(IdentityReviewRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface IIdentityReviewEventSink { ValueTask PublishAsync(IdentityReviewEvent domainEvent, CancellationToken cancellationToken); }