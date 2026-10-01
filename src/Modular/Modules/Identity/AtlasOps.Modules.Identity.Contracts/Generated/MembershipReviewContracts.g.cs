namespace AtlasOps.Modules.Identity.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum MembershipReviewState { Draft, Active, Paused, Completed, Archived }
public sealed record MembershipReviewRecord(Guid Id, string Name, string Owner, MembershipReviewState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record MembershipReviewCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record MembershipReviewEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record MembershipReviewQuery(string? SearchText, MembershipReviewState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record MembershipReviewPage(IReadOnlyList<MembershipReviewRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record MembershipReviewMutation(bool Succeeded, string Code, string Message, MembershipReviewRecord? Record, MembershipReviewEvent? Event);
public interface IMembershipReviewRepository
{
    ValueTask<MembershipReviewRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<MembershipReviewPage> QueryAsync(MembershipReviewQuery query, CancellationToken cancellationToken);
    ValueTask<MembershipReviewMutation> SaveAsync(MembershipReviewRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface IMembershipReviewEventSink { ValueTask PublishAsync(MembershipReviewEvent domainEvent, CancellationToken cancellationToken); }