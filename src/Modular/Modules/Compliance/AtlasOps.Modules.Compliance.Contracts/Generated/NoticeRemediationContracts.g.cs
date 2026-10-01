namespace AtlasOps.Modules.Compliance.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum NoticeRemediationState { Draft, Active, Paused, Completed, Archived }
public sealed record NoticeRemediationRecord(Guid Id, string Name, string Owner, NoticeRemediationState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record NoticeRemediationCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record NoticeRemediationEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record NoticeRemediationQuery(string? SearchText, NoticeRemediationState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record NoticeRemediationPage(IReadOnlyList<NoticeRemediationRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record NoticeRemediationMutation(bool Succeeded, string Code, string Message, NoticeRemediationRecord? Record, NoticeRemediationEvent? Event);
public interface INoticeRemediationRepository
{
    ValueTask<NoticeRemediationRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<NoticeRemediationPage> QueryAsync(NoticeRemediationQuery query, CancellationToken cancellationToken);
    ValueTask<NoticeRemediationMutation> SaveAsync(NoticeRemediationRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface INoticeRemediationEventSink { ValueTask PublishAsync(NoticeRemediationEvent domainEvent, CancellationToken cancellationToken); }