namespace AtlasOps.Modules.Identity.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum TeamReviewState { Draft, Active, Paused, Completed, Archived }
public sealed record TeamReviewRecord(Guid Id, string Name, string Owner, TeamReviewState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record TeamReviewCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record TeamReviewEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record TeamReviewQuery(string? SearchText, TeamReviewState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record TeamReviewPage(IReadOnlyList<TeamReviewRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record TeamReviewMutation(bool Succeeded, string Code, string Message, TeamReviewRecord? Record, TeamReviewEvent? Event);
public interface ITeamReviewRepository
{
    ValueTask<TeamReviewRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<TeamReviewPage> QueryAsync(TeamReviewQuery query, CancellationToken cancellationToken);
    ValueTask<TeamReviewMutation> SaveAsync(TeamReviewRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface ITeamReviewEventSink { ValueTask PublishAsync(TeamReviewEvent domainEvent, CancellationToken cancellationToken); }