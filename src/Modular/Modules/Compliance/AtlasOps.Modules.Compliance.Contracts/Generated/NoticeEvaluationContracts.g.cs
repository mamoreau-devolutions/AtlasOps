namespace AtlasOps.Modules.Compliance.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum NoticeEvaluationState { Draft, Active, Paused, Completed, Archived }
public sealed record NoticeEvaluationRecord(Guid Id, string Name, string Owner, NoticeEvaluationState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record NoticeEvaluationCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record NoticeEvaluationEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record NoticeEvaluationQuery(string? SearchText, NoticeEvaluationState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record NoticeEvaluationPage(IReadOnlyList<NoticeEvaluationRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record NoticeEvaluationMutation(bool Succeeded, string Code, string Message, NoticeEvaluationRecord? Record, NoticeEvaluationEvent? Event);
public interface INoticeEvaluationRepository
{
    ValueTask<NoticeEvaluationRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<NoticeEvaluationPage> QueryAsync(NoticeEvaluationQuery query, CancellationToken cancellationToken);
    ValueTask<NoticeEvaluationMutation> SaveAsync(NoticeEvaluationRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface INoticeEvaluationEventSink { ValueTask PublishAsync(NoticeEvaluationEvent domainEvent, CancellationToken cancellationToken); }