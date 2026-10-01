namespace AtlasOps.Modules.Localization.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum TimeZoneFallbackState { Draft, Active, Paused, Completed, Archived }
public sealed record TimeZoneFallbackRecord(Guid Id, string Name, string Owner, TimeZoneFallbackState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record TimeZoneFallbackCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record TimeZoneFallbackEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record TimeZoneFallbackQuery(string? SearchText, TimeZoneFallbackState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record TimeZoneFallbackPage(IReadOnlyList<TimeZoneFallbackRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record TimeZoneFallbackMutation(bool Succeeded, string Code, string Message, TimeZoneFallbackRecord? Record, TimeZoneFallbackEvent? Event);
public interface ITimeZoneFallbackRepository
{
    ValueTask<TimeZoneFallbackRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<TimeZoneFallbackPage> QueryAsync(TimeZoneFallbackQuery query, CancellationToken cancellationToken);
    ValueTask<TimeZoneFallbackMutation> SaveAsync(TimeZoneFallbackRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface ITimeZoneFallbackEventSink { ValueTask PublishAsync(TimeZoneFallbackEvent domainEvent, CancellationToken cancellationToken); }