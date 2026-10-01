namespace AtlasOps.Modules.Geospatial.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum RouteReportingState { Draft, Active, Paused, Completed, Archived }
public sealed record RouteReportingRecord(Guid Id, string Name, string Owner, RouteReportingState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record RouteReportingCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record RouteReportingEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record RouteReportingQuery(string? SearchText, RouteReportingState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record RouteReportingPage(IReadOnlyList<RouteReportingRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record RouteReportingMutation(bool Succeeded, string Code, string Message, RouteReportingRecord? Record, RouteReportingEvent? Event);
public interface IRouteReportingRepository
{
    ValueTask<RouteReportingRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<RouteReportingPage> QueryAsync(RouteReportingQuery query, CancellationToken cancellationToken);
    ValueTask<RouteReportingMutation> SaveAsync(RouteReportingRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface IRouteReportingEventSink { ValueTask PublishAsync(RouteReportingEvent domainEvent, CancellationToken cancellationToken); }