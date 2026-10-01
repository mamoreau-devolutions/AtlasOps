namespace AtlasOps.Modules.Geography.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum LocationReportingState { Draft, Active, Paused, Completed, Archived }
public sealed record LocationReportingRecord(Guid Id, string Name, string Owner, LocationReportingState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record LocationReportingCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record LocationReportingEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record LocationReportingQuery(string? SearchText, LocationReportingState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record LocationReportingPage(IReadOnlyList<LocationReportingRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record LocationReportingMutation(bool Succeeded, string Code, string Message, LocationReportingRecord? Record, LocationReportingEvent? Event);
public interface ILocationReportingRepository
{
    ValueTask<LocationReportingRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<LocationReportingPage> QueryAsync(LocationReportingQuery query, CancellationToken cancellationToken);
    ValueTask<LocationReportingMutation> SaveAsync(LocationReportingRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface ILocationReportingEventSink { ValueTask PublishAsync(LocationReportingEvent domainEvent, CancellationToken cancellationToken); }