namespace AtlasOps.Modules.CloudEconomics.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum RegionSizingState { Draft, Active, Paused, Completed, Archived }
public sealed record RegionSizingRecord(Guid Id, string Name, string Owner, RegionSizingState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record RegionSizingCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record RegionSizingEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record RegionSizingQuery(string? SearchText, RegionSizingState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record RegionSizingPage(IReadOnlyList<RegionSizingRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record RegionSizingMutation(bool Succeeded, string Code, string Message, RegionSizingRecord? Record, RegionSizingEvent? Event);
public interface IRegionSizingRepository
{
    ValueTask<RegionSizingRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<RegionSizingPage> QueryAsync(RegionSizingQuery query, CancellationToken cancellationToken);
    ValueTask<RegionSizingMutation> SaveAsync(RegionSizingRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface IRegionSizingEventSink { ValueTask PublishAsync(RegionSizingEvent domainEvent, CancellationToken cancellationToken); }