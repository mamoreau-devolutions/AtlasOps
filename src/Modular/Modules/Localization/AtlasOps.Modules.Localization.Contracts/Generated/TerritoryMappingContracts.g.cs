namespace AtlasOps.Modules.Localization.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum TerritoryMappingState { Draft, Active, Paused, Completed, Archived }
public sealed record TerritoryMappingRecord(Guid Id, string Name, string Owner, TerritoryMappingState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record TerritoryMappingCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record TerritoryMappingEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record TerritoryMappingQuery(string? SearchText, TerritoryMappingState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record TerritoryMappingPage(IReadOnlyList<TerritoryMappingRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record TerritoryMappingMutation(bool Succeeded, string Code, string Message, TerritoryMappingRecord? Record, TerritoryMappingEvent? Event);
public interface ITerritoryMappingRepository
{
    ValueTask<TerritoryMappingRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<TerritoryMappingPage> QueryAsync(TerritoryMappingQuery query, CancellationToken cancellationToken);
    ValueTask<TerritoryMappingMutation> SaveAsync(TerritoryMappingRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface ITerritoryMappingEventSink { ValueTask PublishAsync(TerritoryMappingEvent domainEvent, CancellationToken cancellationToken); }