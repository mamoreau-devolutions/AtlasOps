namespace AtlasOps.Modules.Geography.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum RegionSearchState { Draft, Active, Paused, Completed, Archived }
public sealed record RegionSearchRecord(Guid Id, string Name, string Owner, RegionSearchState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record RegionSearchCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record RegionSearchEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record RegionSearchQuery(string? SearchText, RegionSearchState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record RegionSearchPage(IReadOnlyList<RegionSearchRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record RegionSearchMutation(bool Succeeded, string Code, string Message, RegionSearchRecord? Record, RegionSearchEvent? Event);
public interface IRegionSearchRepository
{
    ValueTask<RegionSearchRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<RegionSearchPage> QueryAsync(RegionSearchQuery query, CancellationToken cancellationToken);
    ValueTask<RegionSearchMutation> SaveAsync(RegionSearchRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface IRegionSearchEventSink { ValueTask PublishAsync(RegionSearchEvent domainEvent, CancellationToken cancellationToken); }