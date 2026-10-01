namespace AtlasOps.Modules.Localization.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum TerritoryFormattingState { Draft, Active, Paused, Completed, Archived }
public sealed record TerritoryFormattingRecord(Guid Id, string Name, string Owner, TerritoryFormattingState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record TerritoryFormattingCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record TerritoryFormattingEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record TerritoryFormattingQuery(string? SearchText, TerritoryFormattingState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record TerritoryFormattingPage(IReadOnlyList<TerritoryFormattingRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record TerritoryFormattingMutation(bool Succeeded, string Code, string Message, TerritoryFormattingRecord? Record, TerritoryFormattingEvent? Event);
public interface ITerritoryFormattingRepository
{
    ValueTask<TerritoryFormattingRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<TerritoryFormattingPage> QueryAsync(TerritoryFormattingQuery query, CancellationToken cancellationToken);
    ValueTask<TerritoryFormattingMutation> SaveAsync(TerritoryFormattingRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface ITerritoryFormattingEventSink { ValueTask PublishAsync(TerritoryFormattingEvent domainEvent, CancellationToken cancellationToken); }