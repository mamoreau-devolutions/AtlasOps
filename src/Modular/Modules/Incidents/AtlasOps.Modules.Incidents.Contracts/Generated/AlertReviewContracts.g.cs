namespace AtlasOps.Modules.Incidents.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum AlertReviewState { Draft, Active, Paused, Completed, Archived }
public sealed record AlertReviewRecord(Guid Id, string Name, string Owner, AlertReviewState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record AlertReviewCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record AlertReviewEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record AlertReviewQuery(string? SearchText, AlertReviewState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record AlertReviewPage(IReadOnlyList<AlertReviewRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record AlertReviewMutation(bool Succeeded, string Code, string Message, AlertReviewRecord? Record, AlertReviewEvent? Event);
public interface IAlertReviewRepository
{
    ValueTask<AlertReviewRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<AlertReviewPage> QueryAsync(AlertReviewQuery query, CancellationToken cancellationToken);
    ValueTask<AlertReviewMutation> SaveAsync(AlertReviewRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface IAlertReviewEventSink { ValueTask PublishAsync(AlertReviewEvent domainEvent, CancellationToken cancellationToken); }