namespace AtlasOps.Modules.Geospatial.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum GeometryIntersectionState { Draft, Active, Paused, Completed, Archived }
public sealed record GeometryIntersectionRecord(Guid Id, string Name, string Owner, GeometryIntersectionState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record GeometryIntersectionCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record GeometryIntersectionEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record GeometryIntersectionQuery(string? SearchText, GeometryIntersectionState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record GeometryIntersectionPage(IReadOnlyList<GeometryIntersectionRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record GeometryIntersectionMutation(bool Succeeded, string Code, string Message, GeometryIntersectionRecord? Record, GeometryIntersectionEvent? Event);
public interface IGeometryIntersectionRepository
{
    ValueTask<GeometryIntersectionRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<GeometryIntersectionPage> QueryAsync(GeometryIntersectionQuery query, CancellationToken cancellationToken);
    ValueTask<GeometryIntersectionMutation> SaveAsync(GeometryIntersectionRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface IGeometryIntersectionEventSink { ValueTask PublishAsync(GeometryIntersectionEvent domainEvent, CancellationToken cancellationToken); }