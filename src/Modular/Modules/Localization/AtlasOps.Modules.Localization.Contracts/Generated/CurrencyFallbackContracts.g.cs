namespace AtlasOps.Modules.Localization.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum CurrencyFallbackState { Draft, Active, Paused, Completed, Archived }
public sealed record CurrencyFallbackRecord(Guid Id, string Name, string Owner, CurrencyFallbackState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record CurrencyFallbackCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record CurrencyFallbackEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record CurrencyFallbackQuery(string? SearchText, CurrencyFallbackState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record CurrencyFallbackPage(IReadOnlyList<CurrencyFallbackRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record CurrencyFallbackMutation(bool Succeeded, string Code, string Message, CurrencyFallbackRecord? Record, CurrencyFallbackEvent? Event);
public interface ICurrencyFallbackRepository
{
    ValueTask<CurrencyFallbackRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<CurrencyFallbackPage> QueryAsync(CurrencyFallbackQuery query, CancellationToken cancellationToken);
    ValueTask<CurrencyFallbackMutation> SaveAsync(CurrencyFallbackRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface ICurrencyFallbackEventSink { ValueTask PublishAsync(CurrencyFallbackEvent domainEvent, CancellationToken cancellationToken); }