namespace AtlasOps.Modules.Localization.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum CurrencyFormattingState { Draft, Active, Paused, Completed, Archived }
public sealed record CurrencyFormattingRecord(Guid Id, string Name, string Owner, CurrencyFormattingState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record CurrencyFormattingCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record CurrencyFormattingEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record CurrencyFormattingQuery(string? SearchText, CurrencyFormattingState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record CurrencyFormattingPage(IReadOnlyList<CurrencyFormattingRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record CurrencyFormattingMutation(bool Succeeded, string Code, string Message, CurrencyFormattingRecord? Record, CurrencyFormattingEvent? Event);
public interface ICurrencyFormattingRepository
{
    ValueTask<CurrencyFormattingRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<CurrencyFormattingPage> QueryAsync(CurrencyFormattingQuery query, CancellationToken cancellationToken);
    ValueTask<CurrencyFormattingMutation> SaveAsync(CurrencyFormattingRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface ICurrencyFormattingEventSink { ValueTask PublishAsync(CurrencyFormattingEvent domainEvent, CancellationToken cancellationToken); }