namespace AtlasOps.Modules.Localization.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum TerritoryFallbackState { Draft, Active, Paused, Completed, Archived }
public sealed record TerritoryFallbackRecord(Guid Id, string Name, string Owner, TerritoryFallbackState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record TerritoryFallbackCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record TerritoryFallbackEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record TerritoryFallbackQuery(string? SearchText, TerritoryFallbackState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record TerritoryFallbackPage(IReadOnlyList<TerritoryFallbackRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record TerritoryFallbackMutation(bool Succeeded, string Code, string Message, TerritoryFallbackRecord? Record, TerritoryFallbackEvent? Event);
public interface ITerritoryFallbackRepository
{
    ValueTask<TerritoryFallbackRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<TerritoryFallbackPage> QueryAsync(TerritoryFallbackQuery query, CancellationToken cancellationToken);
    ValueTask<TerritoryFallbackMutation> SaveAsync(TerritoryFallbackRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface ITerritoryFallbackEventSink { ValueTask PublishAsync(TerritoryFallbackEvent domainEvent, CancellationToken cancellationToken); }