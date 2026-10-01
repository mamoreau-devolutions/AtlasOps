namespace AtlasOps.Modules.Compliance.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum NoticeApprovalState { Draft, Active, Paused, Completed, Archived }
public sealed record NoticeApprovalRecord(Guid Id, string Name, string Owner, NoticeApprovalState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record NoticeApprovalCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record NoticeApprovalEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record NoticeApprovalQuery(string? SearchText, NoticeApprovalState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record NoticeApprovalPage(IReadOnlyList<NoticeApprovalRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record NoticeApprovalMutation(bool Succeeded, string Code, string Message, NoticeApprovalRecord? Record, NoticeApprovalEvent? Event);
public interface INoticeApprovalRepository
{
    ValueTask<NoticeApprovalRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<NoticeApprovalPage> QueryAsync(NoticeApprovalQuery query, CancellationToken cancellationToken);
    ValueTask<NoticeApprovalMutation> SaveAsync(NoticeApprovalRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface INoticeApprovalEventSink { ValueTask PublishAsync(NoticeApprovalEvent domainEvent, CancellationToken cancellationToken); }