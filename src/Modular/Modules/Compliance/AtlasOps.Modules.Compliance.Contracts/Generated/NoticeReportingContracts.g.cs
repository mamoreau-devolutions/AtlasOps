namespace AtlasOps.Modules.Compliance.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum NoticeReportingState { Draft, Active, Paused, Completed, Archived }
public sealed record NoticeReportingRecord(Guid Id, string Name, string Owner, NoticeReportingState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record NoticeReportingCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record NoticeReportingEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record NoticeReportingQuery(string? SearchText, NoticeReportingState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record NoticeReportingPage(IReadOnlyList<NoticeReportingRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record NoticeReportingMutation(bool Succeeded, string Code, string Message, NoticeReportingRecord? Record, NoticeReportingEvent? Event);
public interface INoticeReportingRepository
{
    ValueTask<NoticeReportingRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<NoticeReportingPage> QueryAsync(NoticeReportingQuery query, CancellationToken cancellationToken);
    ValueTask<NoticeReportingMutation> SaveAsync(NoticeReportingRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface INoticeReportingEventSink { ValueTask PublishAsync(NoticeReportingEvent domainEvent, CancellationToken cancellationToken); }