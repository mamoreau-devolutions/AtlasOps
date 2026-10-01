namespace AtlasOps.Modules.Geography.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum RegionComparisonState { Draft, Active, Paused, Completed, Archived }
public sealed record RegionComparisonRecord(Guid Id, string Name, string Owner, RegionComparisonState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record RegionComparisonCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record RegionComparisonEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record RegionComparisonQuery(string? SearchText, RegionComparisonState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record RegionComparisonPage(IReadOnlyList<RegionComparisonRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record RegionComparisonMutation(bool Succeeded, string Code, string Message, RegionComparisonRecord? Record, RegionComparisonEvent? Event);
public interface IRegionComparisonRepository
{
    ValueTask<RegionComparisonRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<RegionComparisonPage> QueryAsync(RegionComparisonQuery query, CancellationToken cancellationToken);
    ValueTask<RegionComparisonMutation> SaveAsync(RegionComparisonRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface IRegionComparisonEventSink { ValueTask PublishAsync(RegionComparisonEvent domainEvent, CancellationToken cancellationToken); }