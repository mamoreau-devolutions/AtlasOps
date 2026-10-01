namespace AtlasOps.Modules.Geospatial.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum GeometryReportingState { Draft, Active, Paused, Completed, Archived }
public sealed record GeometryReportingRecord(Guid Id, string Name, string Owner, GeometryReportingState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record GeometryReportingCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record GeometryReportingEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record GeometryReportingQuery(string? SearchText, GeometryReportingState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record GeometryReportingPage(IReadOnlyList<GeometryReportingRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record GeometryReportingMutation(bool Succeeded, string Code, string Message, GeometryReportingRecord? Record, GeometryReportingEvent? Event);
public interface IGeometryReportingRepository
{
    ValueTask<GeometryReportingRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<GeometryReportingPage> QueryAsync(GeometryReportingQuery query, CancellationToken cancellationToken);
    ValueTask<GeometryReportingMutation> SaveAsync(GeometryReportingRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface IGeometryReportingEventSink { ValueTask PublishAsync(GeometryReportingEvent domainEvent, CancellationToken cancellationToken); }