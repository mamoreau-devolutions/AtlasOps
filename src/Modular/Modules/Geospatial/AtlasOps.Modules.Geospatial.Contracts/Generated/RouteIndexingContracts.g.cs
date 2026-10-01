namespace AtlasOps.Modules.Geospatial.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum RouteIndexingState { Draft, Active, Paused, Completed, Archived }
public sealed record RouteIndexingRecord(Guid Id, string Name, string Owner, RouteIndexingState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record RouteIndexingCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record RouteIndexingEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record RouteIndexingQuery(string? SearchText, RouteIndexingState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record RouteIndexingPage(IReadOnlyList<RouteIndexingRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record RouteIndexingMutation(bool Succeeded, string Code, string Message, RouteIndexingRecord? Record, RouteIndexingEvent? Event);
public interface IRouteIndexingRepository
{
    ValueTask<RouteIndexingRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<RouteIndexingPage> QueryAsync(RouteIndexingQuery query, CancellationToken cancellationToken);
    ValueTask<RouteIndexingMutation> SaveAsync(RouteIndexingRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface IRouteIndexingEventSink { ValueTask PublishAsync(RouteIndexingEvent domainEvent, CancellationToken cancellationToken); }