namespace AtlasOps.Modules.Geography.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum RegionReportingState { Draft, Active, Paused, Completed, Archived }
public sealed record RegionReportingRecord(Guid Id, string Name, string Owner, RegionReportingState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record RegionReportingCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record RegionReportingEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record RegionReportingQuery(string? SearchText, RegionReportingState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record RegionReportingPage(IReadOnlyList<RegionReportingRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record RegionReportingMutation(bool Succeeded, string Code, string Message, RegionReportingRecord? Record, RegionReportingEvent? Event);
public interface IRegionReportingRepository
{
    ValueTask<RegionReportingRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<RegionReportingPage> QueryAsync(RegionReportingQuery query, CancellationToken cancellationToken);
    ValueTask<RegionReportingMutation> SaveAsync(RegionReportingRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface IRegionReportingEventSink { ValueTask PublishAsync(RegionReportingEvent domainEvent, CancellationToken cancellationToken); }