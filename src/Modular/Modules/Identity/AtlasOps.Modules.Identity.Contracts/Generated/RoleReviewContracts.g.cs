namespace AtlasOps.Modules.Identity.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum RoleReviewState { Draft, Active, Paused, Completed, Archived }
public sealed record RoleReviewRecord(Guid Id, string Name, string Owner, RoleReviewState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record RoleReviewCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record RoleReviewEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record RoleReviewQuery(string? SearchText, RoleReviewState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record RoleReviewPage(IReadOnlyList<RoleReviewRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record RoleReviewMutation(bool Succeeded, string Code, string Message, RoleReviewRecord? Record, RoleReviewEvent? Event);
public interface IRoleReviewRepository
{
    ValueTask<RoleReviewRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<RoleReviewPage> QueryAsync(RoleReviewQuery query, CancellationToken cancellationToken);
    ValueTask<RoleReviewMutation> SaveAsync(RoleReviewRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface IRoleReviewEventSink { ValueTask PublishAsync(RoleReviewEvent domainEvent, CancellationToken cancellationToken); }