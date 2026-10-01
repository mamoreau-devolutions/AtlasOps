namespace AtlasOps.Modules.Localization.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum LocaleFallbackState { Draft, Active, Paused, Completed, Archived }
public sealed record LocaleFallbackRecord(Guid Id, string Name, string Owner, LocaleFallbackState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record LocaleFallbackCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record LocaleFallbackEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record LocaleFallbackQuery(string? SearchText, LocaleFallbackState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record LocaleFallbackPage(IReadOnlyList<LocaleFallbackRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record LocaleFallbackMutation(bool Succeeded, string Code, string Message, LocaleFallbackRecord? Record, LocaleFallbackEvent? Event);
public interface ILocaleFallbackRepository
{
    ValueTask<LocaleFallbackRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<LocaleFallbackPage> QueryAsync(LocaleFallbackQuery query, CancellationToken cancellationToken);
    ValueTask<LocaleFallbackMutation> SaveAsync(LocaleFallbackRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface ILocaleFallbackEventSink { ValueTask PublishAsync(LocaleFallbackEvent domainEvent, CancellationToken cancellationToken); }