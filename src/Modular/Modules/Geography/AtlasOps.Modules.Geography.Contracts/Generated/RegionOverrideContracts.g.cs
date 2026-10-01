namespace AtlasOps.Modules.Geography.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum RegionOverrideState { Draft, Active, Paused, Completed, Archived }
public sealed record RegionOverrideRecord(Guid Id, string Name, string Owner, RegionOverrideState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record RegionOverrideCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record RegionOverrideEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record RegionOverrideQuery(string? SearchText, RegionOverrideState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record RegionOverridePage(IReadOnlyList<RegionOverrideRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record RegionOverrideMutation(bool Succeeded, string Code, string Message, RegionOverrideRecord? Record, RegionOverrideEvent? Event);
public interface IRegionOverrideRepository
{
    ValueTask<RegionOverrideRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<RegionOverridePage> QueryAsync(RegionOverrideQuery query, CancellationToken cancellationToken);
    ValueTask<RegionOverrideMutation> SaveAsync(RegionOverrideRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface IRegionOverrideEventSink { ValueTask PublishAsync(RegionOverrideEvent domainEvent, CancellationToken cancellationToken); }