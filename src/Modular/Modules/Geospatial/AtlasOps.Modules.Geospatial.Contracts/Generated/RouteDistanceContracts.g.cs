namespace AtlasOps.Modules.Geospatial.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum RouteDistanceState { Draft, Active, Paused, Completed, Archived }
public sealed record RouteDistanceRecord(Guid Id, string Name, string Owner, RouteDistanceState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record RouteDistanceCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record RouteDistanceEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record RouteDistanceQuery(string? SearchText, RouteDistanceState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record RouteDistancePage(IReadOnlyList<RouteDistanceRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record RouteDistanceMutation(bool Succeeded, string Code, string Message, RouteDistanceRecord? Record, RouteDistanceEvent? Event);
public interface IRouteDistanceRepository
{
    ValueTask<RouteDistanceRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<RouteDistancePage> QueryAsync(RouteDistanceQuery query, CancellationToken cancellationToken);
    ValueTask<RouteDistanceMutation> SaveAsync(RouteDistanceRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface IRouteDistanceEventSink { ValueTask PublishAsync(RouteDistanceEvent domainEvent, CancellationToken cancellationToken); }