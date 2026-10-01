namespace AtlasOps.Modules.Incidents.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum RunbookReviewState { Draft, Active, Paused, Completed, Archived }
public sealed record RunbookReviewRecord(Guid Id, string Name, string Owner, RunbookReviewState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record RunbookReviewCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record RunbookReviewEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record RunbookReviewQuery(string? SearchText, RunbookReviewState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record RunbookReviewPage(IReadOnlyList<RunbookReviewRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record RunbookReviewMutation(bool Succeeded, string Code, string Message, RunbookReviewRecord? Record, RunbookReviewEvent? Event);
public interface IRunbookReviewRepository
{
    ValueTask<RunbookReviewRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<RunbookReviewPage> QueryAsync(RunbookReviewQuery query, CancellationToken cancellationToken);
    ValueTask<RunbookReviewMutation> SaveAsync(RunbookReviewRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface IRunbookReviewEventSink { ValueTask PublishAsync(RunbookReviewEvent domainEvent, CancellationToken cancellationToken); }