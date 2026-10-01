namespace AtlasOps.Modules.Localization.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum LocaleFormattingState { Draft, Active, Paused, Completed, Archived }
public sealed record LocaleFormattingRecord(Guid Id, string Name, string Owner, LocaleFormattingState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record LocaleFormattingCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record LocaleFormattingEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record LocaleFormattingQuery(string? SearchText, LocaleFormattingState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record LocaleFormattingPage(IReadOnlyList<LocaleFormattingRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record LocaleFormattingMutation(bool Succeeded, string Code, string Message, LocaleFormattingRecord? Record, LocaleFormattingEvent? Event);
public interface ILocaleFormattingRepository
{
    ValueTask<LocaleFormattingRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<LocaleFormattingPage> QueryAsync(LocaleFormattingQuery query, CancellationToken cancellationToken);
    ValueTask<LocaleFormattingMutation> SaveAsync(LocaleFormattingRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface ILocaleFormattingEventSink { ValueTask PublishAsync(LocaleFormattingEvent domainEvent, CancellationToken cancellationToken); }