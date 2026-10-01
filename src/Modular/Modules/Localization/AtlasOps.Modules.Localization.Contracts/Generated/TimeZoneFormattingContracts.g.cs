namespace AtlasOps.Modules.Localization.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum TimeZoneFormattingState { Draft, Active, Paused, Completed, Archived }
public sealed record TimeZoneFormattingRecord(Guid Id, string Name, string Owner, TimeZoneFormattingState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record TimeZoneFormattingCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record TimeZoneFormattingEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record TimeZoneFormattingQuery(string? SearchText, TimeZoneFormattingState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record TimeZoneFormattingPage(IReadOnlyList<TimeZoneFormattingRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record TimeZoneFormattingMutation(bool Succeeded, string Code, string Message, TimeZoneFormattingRecord? Record, TimeZoneFormattingEvent? Event);
public interface ITimeZoneFormattingRepository
{
    ValueTask<TimeZoneFormattingRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<TimeZoneFormattingPage> QueryAsync(TimeZoneFormattingQuery query, CancellationToken cancellationToken);
    ValueTask<TimeZoneFormattingMutation> SaveAsync(TimeZoneFormattingRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface ITimeZoneFormattingEventSink { ValueTask PublishAsync(TimeZoneFormattingEvent domainEvent, CancellationToken cancellationToken); }