namespace AtlasOps.Modules.Localization.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum LanguageFallbackState { Draft, Active, Paused, Completed, Archived }
public sealed record LanguageFallbackRecord(Guid Id, string Name, string Owner, LanguageFallbackState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record LanguageFallbackCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record LanguageFallbackEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record LanguageFallbackQuery(string? SearchText, LanguageFallbackState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record LanguageFallbackPage(IReadOnlyList<LanguageFallbackRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record LanguageFallbackMutation(bool Succeeded, string Code, string Message, LanguageFallbackRecord? Record, LanguageFallbackEvent? Event);
public interface ILanguageFallbackRepository
{
    ValueTask<LanguageFallbackRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<LanguageFallbackPage> QueryAsync(LanguageFallbackQuery query, CancellationToken cancellationToken);
    ValueTask<LanguageFallbackMutation> SaveAsync(LanguageFallbackRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface ILanguageFallbackEventSink { ValueTask PublishAsync(LanguageFallbackEvent domainEvent, CancellationToken cancellationToken); }