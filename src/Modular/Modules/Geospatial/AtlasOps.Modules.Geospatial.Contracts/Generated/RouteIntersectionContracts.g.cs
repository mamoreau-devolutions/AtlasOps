namespace AtlasOps.Modules.Geospatial.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum RouteIntersectionState { Draft, Active, Paused, Completed, Archived }
public sealed record RouteIntersectionRecord(Guid Id, string Name, string Owner, RouteIntersectionState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record RouteIntersectionCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record RouteIntersectionEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record RouteIntersectionQuery(string? SearchText, RouteIntersectionState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record RouteIntersectionPage(IReadOnlyList<RouteIntersectionRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record RouteIntersectionMutation(bool Succeeded, string Code, string Message, RouteIntersectionRecord? Record, RouteIntersectionEvent? Event);
public interface IRouteIntersectionRepository
{
    ValueTask<RouteIntersectionRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<RouteIntersectionPage> QueryAsync(RouteIntersectionQuery query, CancellationToken cancellationToken);
    ValueTask<RouteIntersectionMutation> SaveAsync(RouteIntersectionRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface IRouteIntersectionEventSink { ValueTask PublishAsync(RouteIntersectionEvent domainEvent, CancellationToken cancellationToken); }