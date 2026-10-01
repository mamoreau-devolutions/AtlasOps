namespace AtlasOps.Modules.Compliance.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum NoticeDetectionState { Draft, Active, Paused, Completed, Archived }
public sealed record NoticeDetectionRecord(Guid Id, string Name, string Owner, NoticeDetectionState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record NoticeDetectionCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record NoticeDetectionEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record NoticeDetectionQuery(string? SearchText, NoticeDetectionState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record NoticeDetectionPage(IReadOnlyList<NoticeDetectionRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record NoticeDetectionMutation(bool Succeeded, string Code, string Message, NoticeDetectionRecord? Record, NoticeDetectionEvent? Event);
public interface INoticeDetectionRepository
{
    ValueTask<NoticeDetectionRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<NoticeDetectionPage> QueryAsync(NoticeDetectionQuery query, CancellationToken cancellationToken);
    ValueTask<NoticeDetectionMutation> SaveAsync(NoticeDetectionRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface INoticeDetectionEventSink { ValueTask PublishAsync(NoticeDetectionEvent domainEvent, CancellationToken cancellationToken); }