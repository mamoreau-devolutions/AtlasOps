namespace AtlasOps.Modules.CloudEconomics.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum RegionForecastingState { Draft, Active, Paused, Completed, Archived }
public sealed record RegionForecastingRecord(Guid Id, string Name, string Owner, RegionForecastingState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record RegionForecastingCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record RegionForecastingEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record RegionForecastingQuery(string? SearchText, RegionForecastingState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record RegionForecastingPage(IReadOnlyList<RegionForecastingRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record RegionForecastingMutation(bool Succeeded, string Code, string Message, RegionForecastingRecord? Record, RegionForecastingEvent? Event);
public interface IRegionForecastingRepository
{
    ValueTask<RegionForecastingRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<RegionForecastingPage> QueryAsync(RegionForecastingQuery query, CancellationToken cancellationToken);
    ValueTask<RegionForecastingMutation> SaveAsync(RegionForecastingRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface IRegionForecastingEventSink { ValueTask PublishAsync(RegionForecastingEvent domainEvent, CancellationToken cancellationToken); }