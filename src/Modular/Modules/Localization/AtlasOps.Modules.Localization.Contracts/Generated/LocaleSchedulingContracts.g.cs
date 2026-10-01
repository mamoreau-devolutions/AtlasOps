namespace AtlasOps.Modules.Localization.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum LocaleSchedulingState { Draft, Active, Paused, Completed, Archived }
public sealed record LocaleSchedulingRecord(Guid Id, string Name, string Owner, LocaleSchedulingState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record LocaleSchedulingCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record LocaleSchedulingEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record LocaleSchedulingQuery(string? SearchText, LocaleSchedulingState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record LocaleSchedulingPage(IReadOnlyList<LocaleSchedulingRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record LocaleSchedulingMutation(bool Succeeded, string Code, string Message, LocaleSchedulingRecord? Record, LocaleSchedulingEvent? Event);
public interface ILocaleSchedulingRepository
{
    ValueTask<LocaleSchedulingRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<LocaleSchedulingPage> QueryAsync(LocaleSchedulingQuery query, CancellationToken cancellationToken);
    ValueTask<LocaleSchedulingMutation> SaveAsync(LocaleSchedulingRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface ILocaleSchedulingEventSink { ValueTask PublishAsync(LocaleSchedulingEvent domainEvent, CancellationToken cancellationToken); }