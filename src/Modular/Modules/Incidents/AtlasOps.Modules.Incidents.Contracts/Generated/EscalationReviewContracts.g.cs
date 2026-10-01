namespace AtlasOps.Modules.Incidents.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum EscalationReviewState { Draft, Active, Paused, Completed, Archived }
public sealed record EscalationReviewRecord(Guid Id, string Name, string Owner, EscalationReviewState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record EscalationReviewCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record EscalationReviewEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record EscalationReviewQuery(string? SearchText, EscalationReviewState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record EscalationReviewPage(IReadOnlyList<EscalationReviewRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record EscalationReviewMutation(bool Succeeded, string Code, string Message, EscalationReviewRecord? Record, EscalationReviewEvent? Event);
public interface IEscalationReviewRepository
{
    ValueTask<EscalationReviewRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<EscalationReviewPage> QueryAsync(EscalationReviewQuery query, CancellationToken cancellationToken);
    ValueTask<EscalationReviewMutation> SaveAsync(EscalationReviewRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface IEscalationReviewEventSink { ValueTask PublishAsync(EscalationReviewEvent domainEvent, CancellationToken cancellationToken); }