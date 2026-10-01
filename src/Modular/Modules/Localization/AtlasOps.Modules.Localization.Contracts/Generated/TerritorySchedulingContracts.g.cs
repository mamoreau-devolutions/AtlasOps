namespace AtlasOps.Modules.Localization.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum TerritorySchedulingState { Draft, Active, Paused, Completed, Archived }
public sealed record TerritorySchedulingRecord(Guid Id, string Name, string Owner, TerritorySchedulingState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record TerritorySchedulingCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record TerritorySchedulingEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record TerritorySchedulingQuery(string? SearchText, TerritorySchedulingState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record TerritorySchedulingPage(IReadOnlyList<TerritorySchedulingRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record TerritorySchedulingMutation(bool Succeeded, string Code, string Message, TerritorySchedulingRecord? Record, TerritorySchedulingEvent? Event);
public interface ITerritorySchedulingRepository
{
    ValueTask<TerritorySchedulingRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<TerritorySchedulingPage> QueryAsync(TerritorySchedulingQuery query, CancellationToken cancellationToken);
    ValueTask<TerritorySchedulingMutation> SaveAsync(TerritorySchedulingRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface ITerritorySchedulingEventSink { ValueTask PublishAsync(TerritorySchedulingEvent domainEvent, CancellationToken cancellationToken); }